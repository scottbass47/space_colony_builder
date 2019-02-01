using Server.NetObjects;
using Shared;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class HealthComponent : NetComponent
    {
        private int health;
        public int Health
        {
            get => health;
            set
            {
                health = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            Health = 0;
            netObj.OnUpdate = () => new HealthUpdate { Health = Health };
        }

    }
}
