using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Server.NetObjects;
using Shared;
using Shared.StateChanges;
using UnityEngine;
using Component = ECS.Component;

namespace Server
{
    public class PositionComponent : NetComponent
    {
        private Vector3 pos; 
        public Vector3 Pos
        {
            get => pos;
            set
            {
                pos = value;
                net.Sync();
            }
        }

        private Vector3 vel; 
        public Vector3 Vel
        {
            get => vel;
            set
            {
                vel = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            Pos = Vector3.zero;
            Vel = Vector3.zero;
            netObj.OnUpdate = () => new PositionUpdate { Pos = pos, Vel = vel };
        }
    }
}
