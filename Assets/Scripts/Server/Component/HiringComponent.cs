using ECS;
using Server.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class HiringComponent : Component
    {
        public delegate IJob Hire(Entity worker);
        public Hire OnHire { get; set; }

        public void Reset()
        {
            OnHire = null;
        }
    }
}
