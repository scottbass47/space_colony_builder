using ECS;
using Server.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.Tasks
{
    public sealed class TaskFactory
    {
        private Engine engine;

        public TaskFactory(Engine engine)
        {
            this.engine = engine;
        }

        public Task CreateMiningTask(List<int> oreIDs, Entity player)
        {
            DebugUtils.Assert(oreIDs.Count > 0);

            Task miningTask = new Task($"Mine {oreIDs.Count} ores");
            foreach(int id in oreIDs)
            {
                var ore = engine.GetEntity(id);
                DebugUtils.Assert(engine.IsValid(ore));

                JobPool jobPool = new MiningJobPool(player, ore, player.GetComponent<LevelComponent>().Level);
                miningTask.AddJob(jobPool);
            }
            return miningTask;
        }
    }
}
