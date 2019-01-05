using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    // Marker component for entities that need to send changes over the network
    public class StateUpdateComponent : Component
    {
        public void Reset()
        {
        }
    }
}
