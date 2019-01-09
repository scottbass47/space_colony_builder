using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ResidentComponent : Component
    {
        public Entity House { get; set; }

        public void Reset()
        {
            House = null;
        }
    }
}
