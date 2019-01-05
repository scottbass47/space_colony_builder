using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server 
{
    public class RandomHealthSystem : AbstractSystem
    {
        private System.Random random = new System.Random();
        private WorldStateManager m;

        public RandomHealthSystem(WorldStateManager m) : base(Group.createGroup().All(typeof(HealthComponent)))
        {
            this.m = m;
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var health = entity.GetComponent<HealthComponent>();
                if(random.Next(1000) == 0)
                {
                    health.health.Value = random.Next(100);
                    //Debug.Log($"[Server] health for entity {entity.ID} is {health.health} in Version {m.Version}");
                }
            }
        }
    }
}
