using System;
using System.Collections.Generic;

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

        private bool removing;
        public bool Removing
        {
            get => removing;
            set => removing = value;
        }

        private List<Component> compArr;
        public List<Component> Components => compArr;

        public Entity(int id, Engine engine)
        {
            ID = id;
            this.engine = engine;
            components = new FastMap<Component>();
            componentBits = new Bits();
            compArr = new List<Component>();
        }

        // @Todo What happens when you add a component of a type that already exists?
        public T AddComponent<T>() where T : Component
        {
            var component = Engine.CreateComponent<T>();
            int compIndex = ComponentType.GetIndex(component.GetType()); 
            components.Put(compIndex, component);
            compArr.Add(component);
            componentBits.Set(compIndex, true);
            component.Entity = this;
            component.ComponentAdded();

            // Fire event
            if(ComponentAdded != null) ComponentAdded(ComponentType.Get(component.GetType()), component);

            return component;
        }

        public T RemoveComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            T ret = (T) components.Remove(compIndex);
            compArr.Remove(ret);
            componentBits.Set(compIndex, false);
            ret.ComponentRemoved();

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

        // @Hack Checking the component bits can through ArrayOutOfBounds exception so
        // the temporary workaround is to just surround it in a try catch and pray.
        public bool HasComponent<T>() where T : Component
        {
            int compIndex = ComponentType.GetIndex(typeof(T));
            try
            {
                bool hasComp = componentBits.Get(compIndex);
                return hasComp;
            }
            catch (Exception)
            {
                return false;                
            }
        }
    } 
}