using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.Job
{
    public class MiningJobPool : JobPool
    {
        private Entity player;
        private Entity source;
        private Level level;

        public MiningJobPool(Entity player, Entity source, Level level) 
        {
            this.player = player;
            this.source = source;
            this.level = level;
        }

        public override string GetDescription()
        {
            return $"Mine Ore {source.ID}";
        }

        public override IJob GetJob(Entity worker)
        {
            var ore = source;
            var pos = ore.GetComponent<MapObjectComponent>().Pos;
            var slots = ore.GetComponent<SlotComponent>();

            // Get the next open slot and add the worker to it
            var slotNum = slots.NextSlot();
            slots.AddEntityToSlot(slotNum, worker);

            // Get the location of the slot so we know where to send the worker
            var slotLoc = slots.GetSlotLocation(slotNum);
            var slotPos = pos + new Vector3(slotLoc.x, slotLoc.y, 0);

            var move = new JobMove(level, slotPos);
            var mine = new JobMine(ore, player, slotNum);

            return new JobSequence(move, mine);
        }

        public override bool CanHireWorker()
        {
            return source.GetComponent<SlotComponent>().HasOpenSlot();
        }
    }
}
