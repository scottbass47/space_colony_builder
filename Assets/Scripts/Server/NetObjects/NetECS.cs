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
    public class NetEngine : Engine
    {
        private NetObjectManager nom;

        public NetEngine(NetObjectManager nom) : base()
        {
            this.nom = nom;
            EntityConstructor = (id, engine) => new NetEntity(id, engine, nom);

            EntityAdded += (entity) =>
            {
                // Add to nom here
                if(entity is NetEntity)
                {
                    var netEntity = (NetEntity)entity;
                    netEntity.NetObject.CreateData = GetEntityCreateData(entity);

                    // Add the Entity to the NOM
                    netEntity.AddToNetObjectManager();
                }
            };
        }

        private EntityCreateData GetEntityCreateData(Entity entity)
        {
            var data = new EntityCreateData();
            var dataList = new List<EData>();
            foreach(var comp in entity.Components)
            {
                if(comp is NetComponent)
                {
                    var netComp = (NetComponent)comp;
                    if(netComp.HasEData)
                    {
                        dataList.Add(netComp.EntityData);
                    }
                }
            }
            data.Data = dataList.ToArray();
            return data;
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

        private bool hasEData = false;
        public bool HasEData => hasEData;
        private EData data;
        public EData EntityData
        {
            get => data;
            set
            {
                data = value;
                hasEData = true;
            }
        }

        public override void ComponentAdded()
        {
            netObj = netEntity.NetObject.CreateChild(netObjectType: NetObjectType.COMPONENT);
            netID = netObj.ID;

            OnInit(netObj);
        }

        public override void ComponentRemoved()
        {
            nom.RemoveNetObject(netID);
        }

        // Called AFTER the NetObject has been created but BEFORE it's added to the NetObjectManager
        //
        // Things to do in Init:
        // - Set OnUpdate
        // - Set NetMode
        // - Set EData
        protected virtual void OnInit(NetObject netObj) { }

        public override void OnReset()
        {
            netID = -1;
            hasEData = false;
            data = null;
        }
    }
}
