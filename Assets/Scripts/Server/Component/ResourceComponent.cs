using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using UnityEngine;

namespace Server
{
    public class ResourceComponent : NetComponent
    {
        public NetValue<int> OreAmount;

        public ResourceComponent()
        {
            OreAmount = new NetValue<int>();

            AddNetValue(OreAmount);
        }

        public override SCUpdate CreateChange()
        {
            return new OreUpdate { Amount = OreAmount };
        }

        public override void OnReset()
        {
            OreAmount = new NetValue<int>();

            AddNetValue(OreAmount);
        }
    }
}
