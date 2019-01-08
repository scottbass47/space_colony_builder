using ECS;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class NetSpawnSystem : AbstractSystem
    {
        public NetSpawnSystem() : 
            base(Group.createGroup().All(typeof(NetSpawnComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                Vector3 pos = Vector3.zero;
                if(entity.HasComponent<PositionComponent>())
                {
                    pos = entity.GetComponent<PositionComponent>().Pos.Value;
                }
                else if(entity.HasComponent<MapObjectComponent>())
                {
                    var p = entity.GetComponent<MapObjectComponent>().Pos;
                    pos.x = (int)p.x;
                    pos.y = (int)p.y;
                }

                var world = entity.GetComponent<GlobalComponent>().World;
                var type = entity.GetComponent<EntityTypeComponent>().Type;
                world.ApplyChange(new EntitySpawn { EntityType = type, ID = entity.ID, Pos = pos });
                entity.RemoveComponent<NetSpawnComponent>();
            }
        }
    }
}
