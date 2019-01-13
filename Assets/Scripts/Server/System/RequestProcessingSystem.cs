using ECS;
using Server.Job;
using Shared;
using Shared.SCData;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    Handle(entity, request);
                }
            }
        }

        private void Handle(Entity client, ClientRequest request)
        {
            WorldStateManager wsm = EntityFactory.World;

            if(request.GetType() == typeof(PlaceBuildingRequest))
            {
                var packet = (PlaceBuildingRequest)request;
                var house = EntityFactory.CreateHouse(new Vector3Int { x = (int)packet.Pos.x, y = (int)packet.Pos.y, z = 0 });
                wsm.Engine.AddEntity(house);

                //var colonist = EntityFactory.CreateColonist(wsm.Level);
                //colonist.GetComponent<PositionComponent>().Pos.Value = packet.Pos;
                //Engine.AddEntity(colonist);
            }
            else if(request.GetType() == typeof(AddWorkerRequest))
            {
                AddWorkerRequest workRequest = (AddWorkerRequest)request;
                //var hire = client.AddComponent<HiringComponent>();
                //hire.OnHire = (worker) => 
                //{
                //    var rock = Engine.GetEntity(workRequest.EntityID);
                //    var pos = rock.GetComponent<MapObjectComponent>().Pos;
                //    var slots = rock.GetComponent<SlotComponent>();
                //    var slot = slots.Slots[workRequest.Slot];
                //    var slotPos = pos + new Vector3(slot.x, slot.y, 0);

                //    var move = new JobMove(worker.GetComponent<LevelComponent>().Level, slotPos);
                //    var mine = new JobMine(rock, client);

                //    return new JobSequence(move, mine);
                //}; 
            }
        }
    }
}
