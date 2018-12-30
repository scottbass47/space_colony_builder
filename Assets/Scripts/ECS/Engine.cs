using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    // ECS Todo list:
    // ---------------
    // * Component Mappers (if necessary)
    // * Entity add/remove events
    // * Entity component add/remove events
    // * Group membership updates
    // * Pooled components
    // * Unit tests?

    // Class - ComponentPool
    //
    // * Dict<T, Stack<T>> componentPool
    //
    // * ObtainComponent<T>
    //      Looks for recycled component first, if none exist create a new one
    // * RecycleComponent(T comp)
    //      Adds component to pool
    //
    //  e.AddComponent(engine.CreateComponent<PositionComponent>().Set(1, 2))
    //  e.AddComponent<PositionComponent>().Set(1, 2);
    //
    //  Entity AddComponent method needs to be able to access component pool in Engine
    //
    // Things to consider:
    // 1. Resetting components when they're added back to the pool
    // 2. How can we avoid writing Set boilerplate method for every component

    public class Engine
    {
        private Dictionary<int, Entity> entities;
        private List<AbstractSystem> systems;
        private static int currentID = 0;
        private ComponentPool componentPool;

        public Engine()
        {
            entities = new Dictionary<int, Entity>();
            systems = new List<AbstractSystem>();
            componentPool = new ComponentPool();
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(currentID++, this);
            return entity;
        }

        public void AddEntity(Entity entity)
        {
            // We should probably do something here...
            entities.Add(entity.ID, entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity.ID);
        }

        public Entity GetEntity(int id)
        {
            return entities[id]; // Might throw an error if the entity doesn't exist
        }

        public T CreateComponent<T>() where T : Component
        {
            return componentPool.ObtainComponent<T>();
        }

        public void RecycleComponent<T>(T comp) where T : Component
        {
            componentPool.RecycleComponent<T>(comp);
        }

        public void AddSystem(AbstractSystem system)
        {
            system.Engine = this;
            systems.Add(system);
        }

        public void RemoveSystem(AbstractSystem system)
        {
            systems.Remove(system);
        }

        public List<Entity> GetEntitiesInGroup(Group group)
        {
            List<Entity> ret = new List<Entity>();
            foreach(int id in entities.Keys)
            {
                if(group.Matches(entities[id]))
                {
                    ret.Add(entities[id]);
                }
            }
            return ret;
        }

        public void Update(float delta)
        {
            for(int i = 0; i < systems.Count; i++)
            {
                systems[i].Update(delta);
            }
        }

    }
}
