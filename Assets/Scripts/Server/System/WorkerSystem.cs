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

        private List<Entity> workerPool;

        public WorkerSystem(List<Entity> workerPool) : base(Group.createGroup().All(typeof(WorkerComponent)))
        {
            this.workerPool = workerPool;
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
                        worker.FinishJob();                        
                        workerPool.Add(entity);
                    }
                }
            }
        }

    }
}
