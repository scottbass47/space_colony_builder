using ECS;
using LiteNetLib;
using Shared;
using Shared.SCData;
using Shared.SCPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Server.NetObjects
{
    public class NetObjectManager
    {
        private SCServer server;
        private Dictionary<int, NetObject> netObjects;
        private Pool<NetObject> netObjectPool;
        private HashSet<int> objectsToBeUpdated;
        private HashSet<int> removedObjects;
        private static int nextID = 0;

        public NetObjectManager(SCServer server)
        {
            this.server = server;
            netObjects = new Dictionary<int, NetObject>();
            netObjectPool = new Pool<NetObject>();
            objectsToBeUpdated = new HashSet<int>();
            removedObjects = new HashSet<int>();
        }

        public void SendUpdates()
        {
            foreach(var objID in objectsToBeUpdated)
            {
                if (removedObjects.Contains(objID)) continue;

                DebugUtils.Assert(netObjects.ContainsKey(objID), $"NetObject with ID {objID} doesn't exist!");
                var obj = netObjects[objID];
                var onUpdate = obj.OnUpdate;
                DebugUtils.Assert(onUpdate != null, $"NetObject has no OnUpdate function.");
                var update = onUpdate();
                DebugUtils.Assert(update != null, $"NetObject update is null.");

                // Send update or something
                var method = DeliveryMethod.ReliableOrdered;
                switch(obj.NetMode)
                {
                    case NetMode.LATEST:
                        method = DeliveryMethod.Sequenced;
                        break;
                    case NetMode.IMPORTANT:
                        method = DeliveryMethod.ReliableOrdered;
                        break;
                }
                var updatePacket = new NetUpdatePacket { NetID = obj.ID, Update = update };
                if(obj.SendToAllClients)
                {
                    server.SendToAllClients(updatePacket, method);
                }
                else
                {
                    server.SendToSingleClient(updatePacket, obj.ClientID, method);
                }
            }
            objectsToBeUpdated.Clear();
        }

        public void OnObjectSync(NetObject obj)
        {
            objectsToBeUpdated.Add(obj.ID);
        }

        public NetObject CreateNetObject()
        {
            var obj = netObjectPool.Obtain<NetObject>();
            obj.ID = nextID++;
            obj.Alive = true;
            obj.AddSyncListener(OnObjectSync);
            obj.NetObjectManager = this;
            return obj;
        }

        // @Note: Right now parent-child relationships have to be established BEFORE adding
        // the child to the NetObjectManager so when the packet is sent we can set the ParentID.
        public void AddNetObject(NetObject netObj)
        {
            // By using a queue we can traverse the hierarchy in a BFS order. This guarantees that
            // parents are created before children.
            var queue = new Queue<NetObject>();
            queue.Enqueue(netObj);

            while(queue.Count > 0)
            {
                var obj = queue.Dequeue();

                foreach(var childToAdd in obj.ChildrenToBeAdded)
                {
                    queue.Enqueue(childToAdd);
                }
                obj.ClearChildrenToBeAdded();

                DebugUtils.Assert(!netObjects.ContainsKey(obj.ID), $"NetObject with ID {obj.ID} already exists!");

                // Order matters when creating net objects. Children can't be added before their parents.
                DebugUtils.Assert(!obj.HasParent || netObjects.ContainsKey(obj.ParentID), $"NetObject[{obj.ID}]'s parent with id {obj.ParentID} hasn't been added yet! !");

                netObjects.Add(obj.ID, obj);
                obj.Added = true;
                //Debug.Log($"[Server] - Adding {obj.TypeName} net object {obj.ID} has parent {obj.HasParent}");

                DebugUtils.Assert(obj.NetObjectType != NetObjectType.NOTHING || obj.EntityType != EntityType.NOTHING, "Can't have NOTHING for both fields EntityType and NetObjectType");
                var packet = new NetCreatePacket { NetID = obj.ID, NetObjectType = obj.NetObjectType, EntityType = obj.EntityType, ParentID = obj.HasParent ? obj.ParentID : -1 };
                if(obj.SendToAllClients)
                {
                    server.SendToAllClients(packet);
                }
                else
                {
                    server.SendToSingleClient(packet, obj.ClientID);
                }
            }
        }

        public void RemoveNetObject(int objID)
        {
            DebugUtils.Assert(netObjects.ContainsKey(objID), $"NetObject with ID {objID} doesn't exist! Has this object already been removed?");
            var netObj = netObjects[objID];

            RecursivelyRemoveNetObject(netObj);
        }

        private void RecursivelyRemoveNetObject(NetObject obj, bool childRemoval = false)
        {
            if(obj.HasChildren)
            {
                foreach(var child in obj.Children)
                {
                    RecursivelyRemoveNetObject(Get(child), true);
                }
                obj.Children.Clear();
            }
            if(obj.HasParent && !childRemoval)
            {
                Get(obj.ParentID).RemoveChild(obj.ID);
            }

            var packet = new NetDestroyPacket { NetID = obj.ID };
            if(obj.SendToAllClients)
            {
                server.SendToAllClients(packet);
            }
            else
            {
                server.SendToSingleClient(packet, obj.ClientID);
            }

            obj.RemoveSyncListener(OnObjectSync);
            removedObjects.Add(obj.ID);
            obj.Alive = false;

            netObjects.Remove(obj.ID);
            netObjectPool.Recycle<NetObject>(obj);
        }

        public NetObject Get(int objID)
        {
            DebugUtils.Assert(netObjects.ContainsKey(objID), $"NetObject with ID {objID} doesn't exist!");
            return netObjects[objID];
        }
    }
}
