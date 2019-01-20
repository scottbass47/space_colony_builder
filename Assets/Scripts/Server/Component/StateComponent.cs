using ECS;
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
        public NetValue<int> State;

        public StateComponent()
        {
            State = new NetValue<int>();

            AddNetValue(State);
        }

        public override SCUpdate CreateChange()
        {
            return new StateUpdate { State = State.Value };
        }

        public override void OnResetTemp()
        {
            State = new NetValue<int>();

            AddNetValue(State);
        }
    }
}
