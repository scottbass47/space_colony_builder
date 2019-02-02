using ECS;
using Server.NetObjects;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class StateComponent : NetComponent
    {
        private int state;
        public int State
        {
            get => state;
            set
            {
                state = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            netObj.NetMode = NetMode.IMPORTANT;
            netObj.OnUpdate = () => new StateUpdate { State = State };
        }

    }
}
