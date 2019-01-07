using ECS;
using Server.Job;
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
                    worker.Job.OnUpdate(delta);
                    if(worker.Job.IsFinished())
                    {
                        //Debug.Log("Worker completed task.");
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
            var objects = level.GetObjects().Keys;
            var random = objects.ElementAt(Random.Range(0, objects.Count));
            worker.AssignJob(new JobMove(level, new Vector3(random.x, random.y, 0)), entity);
        }
    }
}
