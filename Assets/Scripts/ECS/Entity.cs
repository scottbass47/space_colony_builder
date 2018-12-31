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

        public event Action<ComponentType, Component> ComponentAdded;
        public event Action<ComponentType, Component> ComponentRemoved;

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
            int compIndex = ComponentType.GetIndex(component.GetType()); 
            components.Put(compIndex, component);
            componentBits.Set(compIndex, true);

            // Fire event
            if(ComponentAdded != null) ComponentAdded(ComponentType.Get(component.GetType()), component);

            return component;
        }

        public T RemoveComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            T ret = (T) components.Remove(compIndex);
            componentBits.Set(compIndex, false);

            Engine.RecycleComponent(ret);

            // Fire event
            if(ComponentRemoved != null) ComponentRemoved(ComponentType.Get(ret.GetType()), ret);

            return ret;
        }

        public T GetComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            return (T) components[compIndex];
        }
    } 
}