using ECS;
using Shared;
using Shared.SCData;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class RequestProcessingSystem : AbstractSystem
    {

        public RequestProcessingSystem() : base(Group.createGroup().All(typeof(ClientComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var client = entity.GetComponent<ClientComponent>();
                
                while(client.Requests.Count > 0)
                {
                    var request = client.Requests.Dequeue();
                    Handle(request);
                }
            }
        }

        private void Handle(ClientRequest request)
        {
            WorldStateManager wsm = EntityFactory.World;

            if(request.GetType() == typeof(PlaceBuildingRequest))
            {
                var packet = (PlaceBuildingRequest)request;
                var house = EntityFactory.CreateHouse(new Vector3Int { x = (int)packet.Pos.x, y = (int)packet.Pos.y, z = 0 });
                wsm.Engine.AddEntity(house);
                wsm.ApplyChange(new EntitySpawn { ID = house.ID, EntityType = EntityType.HOUSE, Pos = packet.Pos });

                var colonist = EntityFactory.CreateColonist(wsm.Level);
                colonist.GetComponent<PositionComponent>().Pos.Value = packet.Pos;
                Engine.AddEntity(colonist);
                wsm.ApplyChange(new EntitySpawn { ID = colonist.ID, EntityType = EntityType.COLONIST, Pos = packet.Pos });
            }
        }
    }
}
