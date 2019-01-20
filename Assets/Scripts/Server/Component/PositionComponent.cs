using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared;
using Shared.StateChanges;
using UnityEngine;
using Component = ECS.Component;

namespace Server
{
    public class PositionComponent : NetComponent
    {
        public NetValue<Vector3> Pos = new NetValue<Vector3>();

        public PositionComponent()
        {
            AddNetValue(Pos);
        }

        public override SCUpdate CreateChange()
        {
            return new PositionUpdate { Pos = Pos };
        }

        public override void OnResetTemp()
        {
            Pos = new NetValue<Vector3>();
            Pos.Value = Vector3.zero;

            AddNetValue(Pos);
        }
    }
}
