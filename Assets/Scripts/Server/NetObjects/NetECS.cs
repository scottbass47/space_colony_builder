using ECS;
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
            nom.AddNetObject(netObj);
        }

        public NetEngine GetNetEngine()
        {
            return Engine as NetEngine; // This is safe because we asserted in the constructor that engine is type NetEngine
        }
    }

    public class NetComponent : Component
    {
        private int netID;
        protected NetEntity netEntity => (NetEntity)Entity;
        protected NetObjectManager nom => netEntity.NetObjectManager;
        protected NetObject net => nom.Get(netID);

        // Called AFTER the NetObject has been created but BEFORE it's added to the NetObjectManager
        public Action<NetObject> OnInit { get; set; }

        public override void ComponentAdded()
        {
            var netObj = nom.CreateNetObject();
            netID = netObj.ID;

            OnInit(netObj);

            NetObject.BindParentChild(netEntity.NetObject, netObj);
            nom.AddNetObject(netObj);
        }

        public override void ComponentRemoved()
        {
            nom.RemoveNetObject(netID);
        }

        public override void OnReset()
        {
            netID = -1;
        }
    }
}
