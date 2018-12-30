using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class AbstractSystem
    {
        public Engine Engine { get; set; }
        private Group group;

        // Note:
        // 1. The group must be set before the system is added to the Engine
        // 2. The group CAN'T change 
        public Group Group
        {
            get => group;
            //set => group = value;
        }

        public List<Entity> Entities => Engine.GetEntitiesInGroup(Group);

        public AbstractSystem(Group group)
        {
            this.group = group;
        }

        public abstract void Update(float delta);
    }
}
