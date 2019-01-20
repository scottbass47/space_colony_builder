using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public abstract class Component : Poolable
    {
        public Entity Entity { get; set; }

        public void Reset()
        {
            OnReset();
            Entity = null;
        }

        // Convenience method, acts as a constructor getting called when the 
        // component is added to a new Entity.
        public virtual void ComponentAdded() { }
        public virtual void ComponentRemoved() { }
        public abstract void OnReset();
    }
}
