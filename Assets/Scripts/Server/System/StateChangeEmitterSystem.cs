using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.StateChanges;
using UnityEngine;

namespace Server 
{
    public class StateChangeEmitterSystem : AbstractSystem
    {
        public StateChangeEmitterSystem() 
            : base(Group.createGroup().All(typeof(StateUpdateComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                foreach(var comp in entity.Components)
                {
                    if(comp is NetComponent)
                    {
                        var net = comp as NetComponent;
                        if(net.HasChanges)
                        {
                            var update = net.CreateChange();
                            var id = entity.ID;

                            //Debug.Log($"[Server] sending update of type {update.GetType()}");
                            entity.GetComponent<GlobalComponent>().World.ApplyChange(new EntityUpdate { ID = id, Update = update });
                            net.ResetChanges();
                        }
                    }
                }
            }
        }
    }
}
