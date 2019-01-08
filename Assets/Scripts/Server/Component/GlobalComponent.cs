using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class GlobalComponent : Component
    {
        public WorldStateManager World { get; set; }
        public Engine Engine { get; set; }

        public void Set(WorldStateManager world, Engine engine)
        {
            World = world;
            Engine = engine;
        }

        public void Reset()
        {
        }
    }
}
