using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;

namespace ECSTestWorld
{
    class PositionComponent : Component
    {
        public float X { get; set; } 
        public float Y { get; set; } 

        public PositionComponent Set(float x, float y)
        {
            X = x;
            Y = y;
            return this;
        }
    }

    class VelocityComponent : Component
    {
        public float VX { get; set; } 
        public float VY { get; set; }

        public VelocityComponent Set(float x, float y)
        {
            VX = x;
            VY = y;
            return this;
        }
    }

    class MovementSystem : AbstractSystem
    {

        public MovementSystem() 
            : base(Group.createGroup().All(typeof(PositionComponent), typeof(VelocityComponent)))
        {
        }

        public override void Update(float delta)
        {
           foreach(Entity entity in Entities)
            {
                var pos = entity.GetComponent<PositionComponent>();
                var vel = entity.GetComponent<VelocityComponent>();

                pos.X += vel.VX * delta;
                pos.Y += vel.VY * delta;

                Console.WriteLine($"Entity[{entity.ID}]: Position - {pos.X}, {pos.Y}");
            } 
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();

            engine.SystemAdded += (system) => Console.WriteLine($"{system.GetType()} added.");
            engine.SystemRemoved += (system) => Console.WriteLine($"{system.GetType()} removed.");
            engine.EntityAdded += (entity) => Console.WriteLine($"Entity with ID {entity.ID} added.");
            engine.EntityRemoved += (entity) => Console.WriteLine($"Entity with ID {entity.ID} removed.");

            Entity e1 = engine.CreateEntity();
            Entity e2 = engine.CreateEntity();

            e1.AddComponent<PositionComponent>().Set(3,4);
            e1.AddComponent<VelocityComponent>().Set(1, 5);

            e1.RemoveComponent<PositionComponent>();
            e1.RemoveComponent<VelocityComponent>();

            e2.AddComponent<PositionComponent>();
            e2.AddComponent<VelocityComponent>();

            e1.AddComponent<PositionComponent>();
            e1.AddComponent<VelocityComponent>();

            engine.AddEntity(e1);
            engine.AddEntity(e2);

            engine.AddSystem(new MovementSystem());

            engine.Update(1);
            Console.ReadKey();
        }
    }
}
