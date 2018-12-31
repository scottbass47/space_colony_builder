using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    // ComponentPool manages unused components to allow for later reassignment.
    public class Pool<T>
    {
        public Dictionary<int, Stack<T>> pool; 
            
        public Pool()
        {
            pool = new Dictionary<int, Stack<T>>();
        }

        //Return a usable item from the pool or create a new instance if there are none available
        public S Obtain<S>() where S : T
        {
            int index = 0;
            if(typeof(Component).IsAssignableFrom(typeof(S))) 
                index = ComponentType.GetIndex(typeof(S));

            if (!pool.TryGetValue(index, out Stack<T> specificPool)) 
            {
                specificPool = new Stack<T>();
                pool.Add(index, specificPool);
            }
            if(specificPool.Count == 0)
            {
                specificPool.Push(Activator.CreateInstance<S>());
            }

            return (S)specificPool.Pop();
        }

        //Place the recyclable item in its respective spot in the pool
        public void Recycle<S>(T item) 
        {
            int index = 0;
            if (typeof(Component).IsAssignableFrom(typeof(S)))
                index = ComponentType.GetIndex(typeof(S));

            if (!pool.TryGetValue(index, out Stack<T> specificPool))
            {
                specificPool = new Stack<T>();
                pool.Add(index, specificPool);
            }

            specificPool.Push(item);
        }
    }
}

