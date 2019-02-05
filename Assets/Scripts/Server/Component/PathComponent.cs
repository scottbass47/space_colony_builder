using Server.NetObjects;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class PathComponent : NetComponent
    {
        private List<PathNode> path;
        public List<PathNode> Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                net.Sync();
            }
        }

        protected override void OnInit(NetObject netObj)
        {
            netObj.NetMode = NetMode.IMPORTANT;
            netObj.OnUpdate = () =>
            {
                var p = new Vector2[Path.Count];
                for(int i = 0; i < p.Length; i++)
                {
                    var node = Path[i];
                    p[i] = new Vector2 { x = node.x, y = node.y };
                }
                return new PathUpdate { Path = p };
            };
        }
    }
}
