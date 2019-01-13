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

        public delegate void JobListener(Entity worker);
        private JobListener OnComplete;
        private Entity me;

        public void AssignJob(IJob job, Entity me, JobListener onComplete = null)
        {
            this.me = me;
            job.SetEntity(me);
            job.Init();
            OnComplete = onComplete;
            this.job = job;
        }

        public void FinishJob()
        {
            if (OnComplete != null) OnComplete(me);
            job.Done();
            job = null;
        }

        public bool AvailableForHire()
        {
            return job == null || !job.RemoveWorkerFromPool;
        }

        public void Reset()
        {
            job = null;
            OnComplete = null;
            me = null;
        }
    }
}
