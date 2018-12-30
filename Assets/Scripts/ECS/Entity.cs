using System;

namespace ECS
{
    public class Entity
    {
        private FastMap<Component> components;
        private Bits componentBits;
        private Engine engine;

        public int ID { get; private set; }
        public Engine Engine => engine; 
        public Bits ComponentBits => componentBits;
        

        public Entity(int id, Engine engine)
        {
            ID = id;
            this.engine = engine;
            components = new FastMap<Component>();
            componentBits = new Bits();
        }

        public T AddComponent<T>() where T : Component
        {
            var component = Engine.CreateComponent<T>();
            int compIndex = ComponentType.GetIndex(component.GetType()); //NUll reference exception
            components.Put(compIndex, component);
            componentBits.Set(compIndex, true);

            return component;
        }

        // Here we need to recycle the component back to the component pool
        // Do we want to return the removed component and put in pool or just put in pool?
        public T RemoveComponent<T>() where T : Component
        {
            //if (!(t is Component)) throw new ArgumentException("Type must be a subtype of Component.");

            int compIndex = ComponentType.GetIndex(typeof(T));
            T ret = (T) components.Remove(compIndex);
            componentBits.Set(compIndex, false);

            Engine.RecycleComponent(ret);
            return ret;
        }

        public T GetComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            return (T) components[compIndex];
        }
    } 
}