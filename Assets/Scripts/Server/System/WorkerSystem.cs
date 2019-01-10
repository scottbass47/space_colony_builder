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
                        // Some jobs are replaceable, and as such don't need to remove
                        // the worker from the worker pool when assigning them.
                        // Replaceable jobs are basically jobs that the AI can perform
                        // but can be taken out of at any time. For example, the colonists
                        // going home after completing their work is a job, but it's low
                        // priority. So while they are doing a job, they shouldn't be taken
                        // out of the worker pool. At anytime while they are going back to 
                        // there home a player could request work to be done and hire that 
                        // worker.

                        var removedFromPool = worker.Job.RemoveWorkerFromPool;
                        worker.FinishJob();                        

                        // Only add the worker back to the pool if they were removed in the first place.
                        if(removedFromPool) workerPool.Add(entity);  
                    }
                }
                else
                {
                    var goHome = jobGoHome(entity);
                    if (goHome != null)
                    {
                        worker.AssignJob(goHome, entity);
                    }
                }
            }
        }

        private JobMove jobGoHome(Entity entity) 
        {
            var resident = entity.GetComponent<ResidentComponent>();
            if (resident.House == null) return null;
            var level = entity.GetComponent<LevelComponent>().Level;
            var dest = resident.House.GetComponent<MapObjectComponent>().Pos;

            if (JobMove.AtDest(entity.GetComponent<PositionComponent>().Pos, dest)) return null;

            var goHome = new JobMove(level, dest);
            goHome.RemoveWorkerFromPool = false;
            return goHome;
        }

    }
}
