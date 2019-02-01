using System.Collections;
using System.Collections.Generic;
using ECS;
using Server.NetObjects;
using Shared;
using UnityEngine;
using Component = ECS.Component;

namespace Server
{
    public class MapObjectComponent : NetComponent
    {
        private Vector3Int pos;
        public Vector3Int Pos
        {
            get => pos;
            set
            {
                pos = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            pos = Vector3Int.zero;
            netObj.OnUpdate = () => new PositionUpdate { Pos = pos };
        }
    }
}