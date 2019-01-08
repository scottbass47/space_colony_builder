using ECS;
using Server.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class WorkerComponent : Component
    {
        public IJob Job => job; 
        private IJob job;

        public void AssignJob(IJob job, Entity entity)
        {
            job.SetEntity(entity);
            job.Init();
            this.job = job;
        }

        public void FinishJob()
        {
            job.Done();
            job = null;
        }

        public void Reset()
        {
            job = null;
        }
    }
}
