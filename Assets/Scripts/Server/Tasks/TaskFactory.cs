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

        public Task CreateMiningTask(List<int> rockIDs, Entity player)
        {
            DebugUtils.Assert(rockIDs.Count > 0);

            Task miningTask = new Task($"Mine {rockIDs.Count} rocks");
            foreach(int id in rockIDs)
            {
                var rock = engine.GetEntity(id);
                DebugUtils.Assert(engine.IsValid(rock));

                JobPool jobPool = new MiningJobPool(player, rock, player.GetComponent<LevelComponent>().Level);
                miningTask.AddJob(jobPool);
            }
            return miningTask;
        }
    }
}
