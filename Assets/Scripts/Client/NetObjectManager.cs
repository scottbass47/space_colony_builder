using Shared.SCData;
using Shared.SCPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Client
{
    public class NetObjectManager : MonoBehaviour
    {
        private Dictionary<int, INetObject> netObjects;
        private Dictionary<NetObjectType, Func<GameObject>> netObjectCreateDict;

        public GameObject taskQueuePrefab;

        private void Awake()
        {
            netObjectCreateDict = new Dictionary<NetObjectType, Func<GameObject>>();

            RegisterNetObjectCreator(NetObjectType.TASK_QUEUE, () => Instantiate(taskQueuePrefab));
        }

        public void RegisterNetObjectCreator(NetObjectType type, Func<GameObject> creator)
        {
            netObjectCreateDict.Add(type, creator);
        }

        private void Start()
        {
            netObjects = new Dictionary<int, INetObject>();

            SetClient(Game.Instance.Client);
        }

        public void SetClient(SCClient client)
        {
            client.AddPacketListener<NetCreatePacket>(OnNetCreate);
            client.AddPacketListener<NetDestroyPacket>(OnNetDestroy);
            client.AddPacketListener<NetUpdatePacket>(OnNetUpdate);
        }

        private bool NetObjectExists(int netID)
        {
            return netObjects.ContainsKey(netID);
        }

        private void OnNetCreate(NetCreatePacket obj)
        {
            //Debug.Log($"[Client] - NetObject {obj.TypeName} created with id {obj.NetID} and parent id {obj.ParentID}");
            DebugUtils.Assert(!NetObjectExists(obj.NetID), $"Net object with id {obj.NetID} already exists on the client.");
            var netObj = new INetObject(this, obj.NetObjectType, obj.EntityType, obj.NetID, obj.CreateData);

            netObjects.Add(obj.NetID, netObj);

            // If the corresponding obj has a parent, then we need to go up the chain
            // of parents and find the first one that has an OnCreate method defined
            // for this type of net object. If we don't find anything, then this net
            // object becomes its own Game Object with the NetObject component.
            if (obj.ParentID != -1)
            {
                netObj.SetParent(obj.ParentID);
                netObjects[obj.ParentID].AddChild(netObj.NetID);

                // Recursively try and find parent that handles creation of this type
                // If no parents have defined a handler, then we need to create a game object
                // with a NetObject component.
                if(TryParentCreate(obj.ParentID, netObj))
                {
                    return;                                        
                }
            }
            netObj.IsGameObject = true;
            var go = CreateGameObject(netObj);
        }

        private bool TryParentCreate(int parentID, INetObject child)
        {
            DebugUtils.Assert(NetObjectExists(parentID), $"NetObject with ID {child.NetID} has a parent with ID {parentID} that doesn't exist.");
            var parent = netObjects[parentID];

            // So here's the deal. Realistically, children are going to be registered in the
            // Start method of the script that's in charge of dealing with the children. If that's
            // the case, then if both the parent and child are created during the same frame, the 
            // child will not have a parent that can handle it's creation until the NEXT frame.
            //
            // The problem becomes even more difficult when we start dealing with nested hierarchies
            // beyond two deep, and throw in dropped packets / packets arriving out of order. 
            //
            // Possible Solutions:
            // -------------------
            // 1. We send Create and Destroy net events on a reliable, sequenced channel to guarantee
            //    all packets arrive and in order. Then we can hold children until N updates after their
            //    parent has been created, where N is the number of levels deep the child is.
            //
            // 2. When a parent registers a child, fire off an event and lookup in a table somewhere
            //    all children that are of that type and have the matching parent. The problem with this
            //    is we want to know whether or not we're creating a GameObject when creating the NetObject.
            //    This means that upon creation children need to know whether or not they have a parent
            //    handler.
            //
            // 3. Parent handlers are registered upon game startup in the NetObjectManager. This way
            //    nothing needs to be called in Start to register children. BUT, what does this even look
            //    like in the code? When you register children handlers you have to define methods for
            //    OnChildCreate, OnChildUpdate, and OnChildDestroy. Without having a reference to the script
            //    that's going to handle these function calls, how do we handle the register?
            //
            // 4. Child registration occurs in Awake, but DOESN'T call the underlying INetObject directly.
            //    Instead, registration is done through the NetObject component, which then stores all the 
            //    registries in a cache and waits for the INetObject to be set. Upon receiving the INetObject,
            //    the NetObject component transfers the cached registries to the INetObject. This way we can
            //    guarantee that by the time the Child's Awake method is called, the parent has already defined
            //    all the children it can handle.
            if(parent.HandlesChild(child.NetObjectType))
            {
                // Call OnCreate for this child
                DebugUtils.Assert(parent.OnChildCreate != null, $"Parent has child of type {child.NetObjectType} registered but does not define an OnChildCreate method.");
                parent.OnChildCreate(child);
                child.SetParentHandler(parent.NetID);
                return true;
            }
            else
            {
                // If the parent has no parent, then we've failed to find a parent to handle this child,
                // so we can return out of the recursive call.
                if (!parent.HasParent) return false;

                // Otherwise we're going to try again with the parent of the parent.
                return TryParentCreate(parent.ParentID, child);
            }
        }

        private GameObject CreateGameObject(INetObject obj)
        {
            GameObject go = null;
            if(netObjectCreateDict.ContainsKey(obj.NetObjectType))
            {
                go = netObjectCreateDict[obj.NetObjectType]();
            }
            else
            {
                // This basically says "if the obj doesn't have a creator registered and also doesn't have an assigned
                // entity type, then that's no good"
                if(obj.EntityType == EntityType.NOTHING)
                {
                    DebugUtils.Assert(false, $"No registered creator for net object of type {obj.NetObjectType}.");
                }
                go = Game.Instance.EntityObjectFactory.CreateEntityObject(obj.EntityType);
            }
            DebugUtils.Assert(go.GetComponent<NetObject>() != null, 
                $"Game object of type {obj.NetObjectType} missing NetObject component. Did you forget to register a child?");

            go.GetComponent<NetObject>().SetNetObject(obj);

            return go;
        }

        private void OnNetUpdate(NetUpdatePacket obj)
        {
            //Debug.Log($"[Client] - NetObject updated with id {obj.NetID}. Update: {obj.Update}");
            DebugUtils.Assert(NetObjectExists(obj.NetID), $"Net object with id {obj.NetID} doesn't exist on the client.");
            var netObj = netObjects[obj.NetID];
            if (netObj.IsGameObject) netObj.OnUpdate(obj.Update);
            else
            {
                if (netObj.HasParentHandler)
                {
                    var parent = netObjects[netObj.ParentHandlerID];
                    parent.OnChildUpdate(netObj, obj.Update);
                }
            }
        }

        private void OnNetDestroy(NetDestroyPacket obj)
        {
            Debug.Log($"[Client] - NetObject destroyed with id {obj.NetID}");
            DebugUtils.Assert(NetObjectExists(obj.NetID), $"Net object with id {obj.NetID} doesn't exist on the client.");

            var netObj = netObjects[obj.NetID];
            netObj.DestroyData = obj.DestroyData;
            // We need to recursively destroy objects and clean up references
            RecursivelyDestroyNetObject(netObj);
        }

        private void RecursivelyDestroyNetObject(INetObject obj, bool cleanupChildReference = true)
        {
            // If the object is a parent, then we want to remove all it's children first.
            // The removal order is important. Leaf nodes need to be removed first,
            // then traverse up the tree removing as we go. 
            if(obj.IsParent)
            {
                foreach(var child in obj.Children)
                {
                    RecursivelyDestroyNetObject(netObjects[child], false);
                }
                obj.RemoveAllChildren();
            }

            if(obj.HasParentHandler)
            {
                netObjects[obj.ParentHandlerID].OnChildDestroy(obj);
            }

            // If the object has a parent, we need to clean up the references
            if(obj.HasParent && cleanupChildReference)
            {
                netObjects[obj.ParentID].RemoveChild(obj.NetID);
            }

            netObjects.Remove(obj.NetID);
            if (obj.IsGameObject) Destroy(obj.GameObject);
        }

        public INetObject GetNetObject(int netID)
        {
            return netObjects[netID];
        }
    }
}