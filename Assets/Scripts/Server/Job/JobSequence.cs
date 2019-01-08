using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;

namespace Server.Job
{
    public class JobSequence : IJob
    {
        private IJob[] jobs;
        private int currentJob = 0;

        public JobSequence(params IJob[] jobs)
        {
            this.jobs = jobs;
        }

        public void Init()
        {
            jobs[currentJob].Init();
        }

        public bool IsFinished()
        {
            return currentJob == jobs.Length;
        }

        public void OnUpdate(float delta)
        {
            jobs[currentJob].OnUpdate(delta);

            if (jobs[currentJob].IsFinished())
            {
                currentJob++;
                if (currentJob < jobs.Length) jobs[currentJob].Init();
            }
        }

        public void SetEntity(Entity entity)
        {
            foreach(var job in jobs)
            {
                job.SetEntity(entity);
            }
        }
    }
}
