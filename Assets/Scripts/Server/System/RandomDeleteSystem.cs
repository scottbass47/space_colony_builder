using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.StateChanges;

namespace Server
{
    public class RandomDeleteSystem : AbstractSystem
    {
        private Random random = new Random();
        private WorldStateManager worldState;

        public RandomDeleteSystem(WorldStateManager worldState)
            : base(Group.createGroup().All(typeof(MapObjectComponent)))
        {
            this.worldState = worldState;
        }

        public override void Update(float delta)
        {
            foreach(Entity entity in Entities)
            {
                if(random.Next(1000) == 0)
                {
                    worldState.ApplyChange(new EntityRemove { ID = entity.ID });
                    Engine.RemoveEntity(entity);
                }
            }
        }
    }
}
