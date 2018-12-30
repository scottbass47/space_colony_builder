using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    // 2. How can we avoid writing Set boilerplate method for every component
    // 3. Difference between new() and activator.createInstance


    // ComponentPool manages unused components to allow for later reassignment.
    public class ComponentPool//Questionable
    {
        public Dictionary<int, Stack<Component>> componentPool; //key is ComponentType ID and returns stack of components with that type
            
        public ComponentPool()
        {
            componentPool = new Dictionary<int, Stack<Component>>();
        }

        public T ObtainComponent<T>() where T : Component
        {
            int index = ComponentType.GetIndex(typeof(T));
            if (!componentPool.TryGetValue(index, out Stack<Component> pool)) 
            {
                pool = new Stack<Component>();
                componentPool.Add(index, pool);
            }
            if(pool.Count == 0)
            {
                pool.Push(Activator.CreateInstance<T>());
            }

            return (T)pool.Pop();
        }

        public void RecycleComponent<T>(T comp) where T : Component
        {
            int index = ComponentType.GetIndex(typeof(T));
            if (!componentPool.TryGetValue(index, out Stack<Component> pool))
            {
                pool = new Stack<Component>();
                componentPool.Add(index, pool);
            }

            pool.Push(comp);
        }
    }
}

