using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class StatsComponent : Component
    {
        public int MineSpeed { get; set; } 
        public float WalkSpeed { get; set; }

        public StatsComponent Set(int mineSpeed, float walkSpeed)
        {
            MineSpeed = mineSpeed;
            WalkSpeed = walkSpeed;
            return this;
        }

        public override void OnReset()
        {
            Set(0, 0);
        }
    }
}
