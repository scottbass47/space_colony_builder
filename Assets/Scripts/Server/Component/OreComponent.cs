using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Server
{
    public class OreComponent : NetComponent
    {
        public NetValue<int> Amount;

        public OreComponent()
        {
            Amount = new NetValue<int>();

            AddNetValue(Amount);
        }

        public override SCUpdate CreateChange()
        {
            return new OreUpdate { Amount = Amount.Value };
        }

        public override void OnReset()
        {
            Amount = new NetValue<int>();

            AddNetValue(Amount);
        }
    }
}
