using ECS;
using Server.NetObjects;
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
        public NetEngine Engine { get; set; }

        public void Set(WorldStateManager world, NetEngine engine)
        {
            World = world;
            Engine = engine;
        }

        public override void OnReset()
        {
        }
    }
}
