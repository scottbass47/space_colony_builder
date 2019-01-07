using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class LevelComponent : Component
    {
        public Level Level { get; set; }

        public void Reset()
        {
            Level = null;
        }
    }
}
