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
        public NetValue<int> Health = new NetValue<int>();

        public HealthComponent()
        {
            AddNetValue(Health);
        }

        public override SCUpdate CreateChange()
        {
            return new HealthUpdate { Health = Health };
        }

        public override void OnReset()
        {
            Health = new NetValue<int>();

            AddNetValue(Health);
        }
    }
}
