using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class StateChangeComponent : Component
    {
        public bool HasChanges { get; set; }

        public void RegisterNetValue<T>(NetValue<T> value)
        {

        }
    }
}
