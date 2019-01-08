﻿using ECS;
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
        public int WalkSpeed { get; set; }

        public StatsComponent Set(int mineSpeed, int walkSpeed)
        {
            MineSpeed = mineSpeed;
            WalkSpeed = walkSpeed;
            return this;
        }

        public void Reset()
        {
            Set(0, 0);
        }
    }
}
