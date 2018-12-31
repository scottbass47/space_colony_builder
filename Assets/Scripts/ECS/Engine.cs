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
    // * Delayed operations (i.e. delay engine operations if engine is in the middle of updating)
    // * Unit tests?

    // Class - ComponentPool
    //
    // Things to consider:
    // 1. Resetting components when they're added back to the pool
    // 2. How can we avoid writing Set boilerplate method for every component
    
    // Delayed Operations:
    //
    // What can't happen while Engine is updating?
    // 1. Add/Remove systems (usually you only add systems when first creating engine)
    //
    // What can't happen while System is updating?
    // 1. Add/Remove entities
    // 2. UpdateGroupMembership 
    // 
    // What can happen always?
    // 1. Add/Remove components 

    public partial class Engine
    {
        private Dictionary<int, Entity> entities;
        private List<AbstractSystem> systems;
        private static int currentID = 0;
        private Pool<Component> componentPool;
        private Pool<Operation> operationPool;
        private bool updating = false;

        public event Action<Entity> EntityAdded;
        public event Action<Entity> EntityRemoved;
        public event Action<AbstractSystem> SystemAdded;
        public event Action<AbstractSystem> SystemRemoved;

        private Dictionary<Group, List<Entity>> groupMembership;

        public Engine()
        {
            entities = new Dictionary<int, Entity>();
            systems = new List<AbstractSystem>();         

            groupMembership = new Dictionary<Group, List<Entity>>();

            EntityAdded += UpdateGroupMembership;

            // @Bug When an entity is removed we don't want to call UpdateGroupMembership
            // (as it is currently) because it might add the entity to groups even though
            // the entity is being removed from the engine.
            EntityRemoved += UpdateGroupMembership;
            SystemAdded += UpdateEntityMembership;
            SystemRemoved += UpdateEntityMembership;

            componentPool = new Pool<Component>();
            operationPool = new Pool<Operation>();

            pendingOps = new Queue<Operation>();
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(currentID++, this);
            return entity;
        }

        public void AddEntity(Entity entity)
        {
            if (updating) pendingOps.Enqueue(CreateOperation().Set(OpType.Add, entity));
            else
            {
                entities.Add(entity.ID, entity);
                EntityAdded(entity);

                Action<ComponentType, Component> ev = (type, comp) =>
                {
                    UpdateGroupMembership(entity);
                };

                entity.ComponentAdded += ev;
                entity.ComponentRemoved += ev;
            }
        }

        //@Memory: recycle all of the entity's components before removing it
        public void RemoveEntity(Entity entity)
        {
            if (updating) pendingOps.Enqueue(CreateOperation().Set(OpType.Remove, entity));
            else
            {
                EntityRemoved(entity);
                entities.Remove(entity.ID);
            }
        }

        // Returns the entity if it exists, null otherwise
        public Entity GetEntity(int id)
        {
            Entity entity;
            entities.TryGetValue(id, out entity);
            return entity;
        }

        public T CreateComponent<T>() where T : Component
        {
            return componentPool.Obtain<T>();
        }

        public void RecycleComponent<T>(T comp) where T : Component
        {
            componentPool.Recycle<T>(comp);
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
            if (updating) pendingOps.Enqueue(CreateOperation().Set(OpType.UpdateGroup, entity));
            else
            {
                foreach (Group group in groupMembership.Keys)
                {
                    if (group.Matches(entity) && !groupMembership[group].Contains(entity))
                    {
                        groupMembership[group].Add(entity);
                    }
                    else if (!group.Matches(entity) && groupMembership[group].Contains(entity))
                    {
                        groupMembership[group].Remove(entity);
                    }
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
                updating = true;
                systems[i].Update(delta);
                updating = false;

                if(hasOps()) ProcessOps();
            }
            
        }

    }
}
