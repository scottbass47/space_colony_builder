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
        public NetValue<int> health = new NetValue<int>();

        public HealthComponent()
        {
            AddNetValue(health);
        }

        public override SCUpdate CreateChange()
        {
            return new HealthUpdate { Health = health };
        }

        public override void Reset()
        {
            base.Reset();
            health = new NetValue<int>();
        }
    }
}
