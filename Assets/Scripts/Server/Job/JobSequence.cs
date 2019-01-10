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

        public override void Init()
        {
            InitJob(jobs[currentJob]);
        }

        public override void Done()
        {
            // The last job's Done will be handled in OnUpdate
        }

        public override void OnUpdate(float delta)
        {
            jobs[currentJob].OnUpdate(delta);

            if (jobs[currentJob].IsFinished())
            {
                jobs[currentJob].Done();
                currentJob++;
                if (currentJob < jobs.Length)
                {
                    InitJob(jobs[currentJob]);
                }
            }
        }

        private void InitJob(IJob job)
        {
            job.Init();
            TerminationConditions.Clear();
            TerminationConditions.AddRange(job.TerminationConditions);
        }

        public override void SetEntity(Entity entity)
        {
            foreach(var job in jobs)
            {
                job.SetEntity(entity);
            }
        }
    }
}
