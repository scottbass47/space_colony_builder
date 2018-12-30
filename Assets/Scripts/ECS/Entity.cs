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

        private event Action<ComponentType, Component> componentAdded;
        private event Action<ComponentType, Component> componentRemoved;
        
        public Entity(int id, Engine engine)
        {
            ID = id;
            this.engine = engine;
            components = new FastMap<Component>();
            componentBits = new Bits();
        }

        // Change this to be AddComponent<T> where T : Component
        // Do something like Engine.Pool.ObtainComponent<T>() in here.
        public void AddComponent(Component component)
        {
            int compIndex = ComponentType.GetIndex(component.GetType()); 
            components[compIndex] = component;
            componentBits.Set(compIndex, true);
        }

        // Here we need to recycle the component back to the component pool
        public T RemoveComponent<T>() where T : Component
        {
            //if (!(t is Component)) throw new ArgumentException("Type must be a subtype of Component.");

            int compIndex = ComponentType.GetIndex(typeof(T));
            T ret = (T) components.Remove(compIndex);
            componentBits.Set(compIndex, false);

            return ret;
        }

        public T GetComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            return (T) components[compIndex];
        }
    } 
}