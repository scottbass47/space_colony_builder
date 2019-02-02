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
        private int id;
        private bool sendToAllClients = true;
        private int clientID;
        private NetMode mode;
        private NetObjectType netType;
        private EntityType entityType = EntityType.NOTHING;
        private bool hasParent;
        private int parent;
        private List<int> children;
        private Func<NetUpdate> update;
        private event SyncListener OnSync;

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
        public NetMode NetMode { get => mode; set => mode = value; }
        public int ClientID => clientID;
        public bool SendToAllClients => sendToAllClients;
        public string TypeName => EntityType == EntityType.NOTHING ? $"{NetObjectType}" : $"{EntityType}";

        public delegate void SyncListener(NetObject obj);

        public NetObject()
        {
            children = new List<int>();
        }

        // Call this whenever changes have been made and the netobject needs to sync
        public void Sync()
        {
            DebugUtils.Assert(Alive, "Can't call sync on a NetObject that's not alive.");
            OnSync(this);
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
            sendToAllClients = false;
            clientID = -1;
            mode = NetMode.LATEST;
            netType = NetObjectType.NOTHING;
            entityType = EntityType.NOTHING;
            parent = -1;
            children.Clear();
            Alive = false;
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
