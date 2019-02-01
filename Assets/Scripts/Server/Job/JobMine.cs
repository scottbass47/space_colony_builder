using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.SCData;
using UnityEngine;
using Utils;

namespace Server.Job
{
    // Component list per entity:
    // Source
    //      - Ore Component (net)
    //      - Slot Component
    // Owner
    //      - Resource Component (net)
    // Worker
    //      - Stats Component
    public class JobMine : IJob
    {
        private Entity source; // thing being mined
        private Entity owner; // who gets the resources
        private Entity worker; // who's doing the mining

        private float elapsed;
        private Engine engine;
        private int slotNum;

        public JobMine(Entity oreSource, Entity player, int slotNum)
        {
            source = oreSource;
            owner = player;
            this.slotNum = slotNum;
           
            // Grab a reference to the engine
            engine = source.GetComponent<GlobalComponent>().Engine;

            AddTerminationCondition(() => !engine.IsValid(source) || source.GetComponent<OreComponent>().Amount == 0);
        }

        public override void Init()
        {
            if(worker.HasComponent<StateComponent>())
            {
                var state = worker.GetComponent<StateComponent>();
                state.State = (int)EntityState.MINING;
            }

            var slots = source.GetComponent<SlotComponent>();
            DebugUtils.Assert(slots.GetEntity(slotNum).ID == worker.ID, $"The worker at slot {slotNum} is not this worker.");
        }

        public override void Done()
        {
            if(worker.HasComponent<StateComponent>())
            {
                var state = worker.GetComponent<StateComponent>();
                state.State = (int)EntityState.IDLE;
            }
            var slots = source.GetComponent<SlotComponent>();
            DebugUtils.Assert(slots.GetEntity(slotNum).ID == worker.ID, $"The worker at slot {slotNum} is not this worker.");
            slots.RemoveFromSlot(slotNum);
        }

        public override void OnUpdate(float delta)
        {
            elapsed += delta;
            if(elapsed >= 1f)
            {
                var ore = source.GetComponent<OreComponent>();
                var stats = worker.GetComponent<StatsComponent>();
                var resources = owner.GetComponent<ResourceComponent>();

                int before = ore.Amount;
                ore.Amount = Math.Max(0, ore.Amount - stats.MineSpeed);
                int after = ore.Amount;
                resources.OreAmount += before - after;

                elapsed = 0;
            }
        }

        public override void SetEntity(Entity entity)
        {
            worker = entity;
        }
    }
}
