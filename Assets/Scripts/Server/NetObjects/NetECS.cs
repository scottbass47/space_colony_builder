using ECS;
using Shared.SCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.NetObjects
{
    public class NetEngine : Engine
    {
        private NetObjectManager nom;

        public NetEngine(NetObjectManager nom) : base()
        {
            this.nom = nom;
            EntityConstructor = (id, engine) => new NetEntity(id, engine, nom);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            nom.SendUpdates();
        }
    }

    public class NetEntity : Entity
    {
        private NetObjectManager nom;
        private NetObject netObj;

        public NetObjectManager NetObjectManager => nom;
        public NetObject NetObject => netObj;

        public NetEntity(int id, Engine engine, NetObjectManager nom) : base(id, engine)
        {
            DebugUtils.Assert(engine.GetType() == typeof(NetEngine), "Can't create a NetEntity without a net engine!");
            this.nom = nom;

            netObj = nom.CreateNetObject();
        }

        public NetEngine GetNetEngine()
        {
            return Engine as NetEngine; // This is safe because we asserted in the constructor that engine is type NetEngine
        }

        public void AddToNetObjectManager()
        {
            nom.AddNetObject(netObj);
        }

        public void RemoveFromNetManager()
        {
            nom.RemoveNetObject(netObj.ID);
        }
    }

    public class NetComponent : Component
    {
        private int netID;
        private NetObject netObj;
        protected NetEntity netEntity => (NetEntity)Entity;
        protected NetObjectManager nom => netEntity.NetObjectManager;
        protected NetObject net => netObj;

        public override void ComponentAdded()
        {
            netObj = nom.CreateNetObject();
            netID = netObj.ID;
            netObj.NetObjectType = NetObjectType.COMPONENT;

            OnInit(netObj);

            NetObject.BindParentChild(netEntity.NetObject, netObj);
            nom.AddNetObject(netObj);
        }

        public override void ComponentRemoved()
        {
            nom.RemoveNetObject(netID);
        }

        // Called AFTER the NetObject has been created but BEFORE it's added to the NetObjectManager
        protected virtual void OnInit(NetObject netObj) { }

        public override void OnReset()
        {
            netID = -1;
        }
    }
}
