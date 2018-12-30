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

        public Group Group
        {
            get => group;
            set => group = value;
        }

        public List<Entity> Entities => Engine.GetEntitiesInGroup(Group);

        public AbstractSystem(Group group)
        {
            Group = group;
        }

        public abstract void Update(float delta);
    }
}
