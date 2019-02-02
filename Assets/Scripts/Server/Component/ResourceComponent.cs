using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.NetObjects;
using Shared;
using UnityEngine;

namespace Server
{
    public class ResourceComponent : NetComponent
    {
        private int oreVal;
        public int OreAmount
        {
            get => oreVal;
            set
            {
                oreVal = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            OreAmount = 0;
            netObj.OnUpdate = () => new OreUpdate { Amount = OreAmount };
        }
    }
}
