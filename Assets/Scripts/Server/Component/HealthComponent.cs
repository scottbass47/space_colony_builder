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
        public NetValue<int> health = new NetValue<int>();

        public HealthComponent()
        {
            AddNetValue(health);
        }

        public override EntityUpdate CreateChange()
        {
            return new HealthUpdate { Health = health };
        }
    }
}
