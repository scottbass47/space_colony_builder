using ECS;
using Shared;
using Shared.SCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.NetObjects
{
    public class NetObject : Poolable
    {
        private NetObjectManager nom;
        private int id;
        private bool sendToAllClients = true;
        private int clientID;
        private NetMode mode;
        private NetObjectType netType = NetObjectType.NOTHING;
        private EntityType entityType = EntityType.NOTHING;
        private bool hasParent;
        private int parent;
        private List<int> children;
        private Func<NetUpdate> update;
        private event SyncListener OnSync;

        // Create / Destroy data
        private CreateData createData;
        private DestroyData destroyData;

        public NetObjectManager NetObjectManager { get => nom; set => nom = value; }
        public int ID { get => id; set => id = value; }
        public int ParentID => parent;
        public bool HasParent => hasParent;
        public Func<NetUpdate> OnUpdate { get => update; set => update = value; }
        public NetObjectType NetObjectType { get => netType; set => netType = value; }
        public EntityType EntityType { get => entityType; set => entityType = value; }
        public bool IsEntity => entityType == EntityType.NOTHING;
        public List<int> Children => children;
        public bool HasChildren => children.Count > 0;
        public bool Alive { get; set; }  // True if the netobject has been created but not destroyed
        public bool Added { get; set; }  // True if the netobject is added to the NetObjectManager 
        public NetMode NetMode { get => mode; set => mode = value; }
        public int ClientID => clientID;
        public bool SendToAllClients => sendToAllClients;
        public string TypeName => EntityType == EntityType.NOTHING ? $"{NetObjectType}" : $"{EntityType}";

        public delegate void SyncListener(NetObject obj);

        private List<NetObject> childrenToBeAdded;
        public List<NetObject> ChildrenToBeAdded => childrenToBeAdded;

        public bool HasCreateData => createData != null;
        public CreateData CreateData { get => createData; set => createData = value; } 
        public bool HasDestroyData => destroyData != null;
        public DestroyData DestroyData { get => destroyData; set => destroyData = value; } 

        public NetObject()
        {
            children = new List<int>();
            childrenToBeAdded = new List<NetObject>();
        }

        // Call this whenever changes have been made and the netobject needs to sync
        public void Sync()
        {
            DebugUtils.Assert(Alive, "Can't call sync on a NetObject that's not alive.");
            OnSync(this);
        }

        // Removes the NetObject from the NetObjectManager sending a Destroy packet to the client
        public void Destroy()
        {
            nom.RemoveNetObject(id);
        }

        public void AddSyncListener(SyncListener listener)
        {
            OnSync += listener; 
        }

        public void RemoveSyncListener(SyncListener listener)
        {
            OnSync -= listener; 
        }

        public void SetSingleClient(int clientID)
        {
            this.clientID = clientID;
            sendToAllClients = false;
        }

        public void Reset()
        {
            id = -1;
            sendToAllClients = true;
            clientID = -1;
            mode = NetMode.LATEST;
            netType = NetObjectType.NOTHING;
            entityType = EntityType.NOTHING;
            parent = 0;
            hasParent = false;
            children.Clear();
            Alive = false;
            Added = false;
            childrenToBeAdded.Clear();
            update = null;
            createData = null;
        }

        public void AddChild(int id)
        {
            children.Add(id);
        }

        public void RemoveChild(int id)
        {
            children.Remove(id);
        }

        public void SetParent(int id)
        {
            hasParent = true;
            parent = id;
        }

        public static void BindParentChild(NetObject parent, NetObject child)
        {
            parent.AddChild(child.ID);
            child.SetParent(parent.id);
        }

        // This will create a new NetObject and bind it to this NetObject in a parent-child relationship.
        // Furthermore, when this NetObject is added to the NetObjectManager, it will add all its children
        // created through this method.
        //
        // IF the parent object is already added to the NetObjectManager, then this method will ALSO add
        // the child.
        public NetObject CreateChild(NetObjectType netObjectType = NetObjectType.NOTHING, EntityType entityType = EntityType.NOTHING, CreateData data = null)
        {
            var childObj = nom.CreateNetObject();
            childObj.NetObjectType = netObjectType;
            childObj.EntityType = entityType;
            childObj.CreateData = data;
            NetObject.BindParentChild(this, childObj);

            if(Added)
            {
                nom.AddNetObject(childObj);
            }
            else
            {
                childrenToBeAdded.Add(childObj);
            }
            return childObj;
        }

        public void ClearChildrenToBeAdded()
        {
            childrenToBeAdded.Clear();
        }

        public override string ToString()
        {
            return $"[{id}]: {TypeName}";
        }
    }

    public enum NetMode
    {
        LATEST,
        IMPORTANT
    }
}
