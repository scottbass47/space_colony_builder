using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.NetObjects;
using Shared;

namespace Server
{
    public class OreComponent : NetComponent
    {
        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            Amount = 0;
            netObj.OnUpdate = () => new OreUpdate { Amount = Amount };
        }
    }
}
