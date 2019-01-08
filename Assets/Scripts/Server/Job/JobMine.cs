using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.SCData;
using UnityEngine;

namespace Server.Job
{
    // Component list per entity:
    // Source
    //      - Ore Component (net)
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

        public JobMine(Entity oreSource, Entity player)
        {
            source = oreSource;
            owner = player;
           
            // Grab a reference to the engine
            engine = source.GetComponent<GlobalComponent>().Engine;
        }

        public void Init()
        {
            if(worker.HasComponent<StateComponent>())
            {
                var state = worker.GetComponent<StateComponent>();
                state.State.Value = (int)EntityState.MINING;
            }
        }

        public bool IsFinished()
        {
            return !engine.IsValid(source) || source.GetComponent<OreComponent>().Amount == 0;
        }

        public void OnUpdate(float delta)
        {
            elapsed += delta;
            if(elapsed >= 1f)
            {
                var ore = source.GetComponent<OreComponent>();
                var stats = worker.GetComponent<StatsComponent>();
                var resources = owner.GetComponent<ResourceComponent>();

                int before = ore.Amount;
                ore.Amount.Value = Math.Max(0, ore.Amount.Value - stats.MineSpeed);
                int after = ore.Amount;
                resources.OreAmount.Value += before - after;

                elapsed = 0;
            }
        }

        public void SetEntity(Entity entity)
        {
            worker = entity;
        }

    }
}
