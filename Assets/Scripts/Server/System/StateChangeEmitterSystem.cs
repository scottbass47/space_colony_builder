using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;

namespace Server 
{
    public class StateChangeEmitterSystem : AbstractSystem
    {
        public StateChangeEmitterSystem() : base(Group.createGroup().All(typeof(StateChangeComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var sc = entity.GetComponent<StateChangeComponent>();

                if(sc.HasChanges)
                {
                    // Send the data!
                }
            }
        }
    }
}
