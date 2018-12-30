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

            engine.AddSystem(new MovementSystem());

            Entity e1 = engine.CreateEntity();
            e1.AddComponent(new PositionComponent { X = 1, Y = 2 });
            e1.AddComponent(new VelocityComponent { VX = -1, VY = -0.5f });
            // e1.AddComponent(engine.CreateComponent<PositionComponent>().Set(1, 2))
            engine.AddEntity(e1);

            engine.Update(1);
            Console.ReadKey();
        }
    }
}
