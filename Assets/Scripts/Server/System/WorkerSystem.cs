using ECS;
using Server.Job;
using Shared.SCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Server
{
    public class WorkerSystem : AbstractSystem
    {

        public WorkerSystem() : base(Group.createGroup().All(typeof(WorkerComponent)))
        {
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var worker = entity.GetComponent<WorkerComponent>();

                if(worker.Job != null)
                {
                    if(!worker.Job.IsFinished())
                    {
                        worker.Job.OnUpdate(delta);
                    }
                    else
                    {
                        giveJob(entity);
                    }
                }
                else
                {
                    giveJob(entity);
                }
            }
        }

        private void giveJob(Entity entity)
        {
            var worker = entity.GetComponent<WorkerComponent>();
            var level = entity.GetComponent<LevelComponent>().Level;
            var positions = level.GetObjects().Keys;

            var rocks = new List<Vector3Int>(positions)
                .FindAll(
                    (pos) => level.ObjectAt(pos).GetComponent<EntityTypeComponent>().Type == EntityType.ROCK
                );

            if (rocks.Count == 0) return;
            var random = rocks[Random.Range(0, rocks.Count)];

            var move = new JobMove(level, new Vector3(random.x, random.y, 0));
            var mine = new JobMine(level.ObjectAt(random), entity.GetComponent<GlobalComponent>().World.GetPlayer(0));

            var jobSeq = new JobSequence(move, mine);
            worker.AssignJob(jobSeq, entity);
        }
    }
}
