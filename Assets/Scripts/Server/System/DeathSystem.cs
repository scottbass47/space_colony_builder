using ECS;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DeathSystem : AbstractSystem
    {

        public DeathSystem() : base(Group.createGroup().One(typeof(OreComponent), typeof(HealthComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                bool dead;
                if(entity.HasComponent<OreComponent>())
                {
                    dead = entity.GetComponent<OreComponent>().Amount <= 0;        
                } 
                else
                {
                    dead = entity.GetComponent<HealthComponent>().Health <= 0;        
                }
                if (dead)
                {
                    Engine.RemoveEntity(entity);
                    entity.GetComponent<GlobalComponent>().World.ApplyChange(new EntityRemove { ID = entity.ID });
                }
            }
        }
    }
}
