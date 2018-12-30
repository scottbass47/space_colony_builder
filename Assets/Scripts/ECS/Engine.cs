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

        private event Action<Entity> EntityAdded;
        private event Action<Entity> EntityRemoved;
        private event Action<AbstractSystem> SystemAdded;
        private event Action<AbstractSystem> SystemRemoved;

        private Dictionary<Group, List<Entity>> groupMembership;

        public Engine()
        {
            entities = new Dictionary<int, Entity>();
            systems = new List<AbstractSystem>();
            groupMembership = new Dictionary<Group, List<Entity>>();

            EntityAdded += UpdateGroupMembership;
            EntityRemoved += UpdateGroupMembership;
            SystemAdded += UpdateEntityMembership;
            SystemRemoved += UpdateEntityMembership;
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(currentID++, this);
            return entity;
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity.ID, entity);
            EntityAdded(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            EntityRemoved(entity);
            entities.Remove(entity.ID);
        }

        // Returns the entity if it exists, null otherwise
        public Entity GetEntity(int id)
        {
            Entity entity;
            entities.TryGetValue(id, out entity);
            return entity;
        }

        public void AddSystem(AbstractSystem system)
        {
            system.Engine = this;
            systems.Add(system);

            // This will throw an error if a group already exists but thats OK
            // because we don't want people adding multiple systems with the same
            // group. It doesn't make sense and it adds unnecessary complications.
            groupMembership.Add(system.Group, new List<Entity>());
            SystemAdded(system);
        }

        public void RemoveSystem(AbstractSystem system)
        {
            SystemRemoved(system);
            systems.Remove(system);
            groupMembership.Remove(system.Group);
        }

        public List<Entity> GetEntitiesInGroup(Group group)
        {
            return groupMembership[group];
        }

        // This method goes through all the groups and updates whether or not
        // they should contain this entity.
        private void UpdateGroupMembership(Entity entity)
        {
            // @Performance I think we can get a performance boost if Entities keep track
            // of what groups they are in using a Set. This would give us O(1) contains
            // instead of the current O(n) contains that a List<> provides.
            foreach(Group group in groupMembership.Keys)
            {
                if(group.Matches(entity) && !groupMembership[group].Contains(entity))
                {
                    groupMembership[group].Add(entity);
                }
                else if(!group.Matches(entity) && groupMembership[group].Contains(entity))
                {
                    groupMembership[group].Remove(entity);
                }
            }
        }

        // This method goes through all the entities in the Engine and adds them
        // to the group if they match.
        private void UpdateEntityMembership(AbstractSystem system)
        {
            var group = system.Group;
            if (!groupMembership.ContainsKey(group)) return;

            // Clear so we don't add duplicates
            groupMembership[group].Clear();

            foreach(Entity entity in entities.Values)
            {
                if(group.Matches(entity))
                {
                    groupMembership[group].Add(entity);
                }
            }
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
