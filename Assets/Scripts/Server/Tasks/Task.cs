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
    public class Task
    {
        private List<JobPool> jobs;
        private string desc;

        public Task(string desc)
        {
            this.desc = desc;
            jobs = new List<JobPool>();
        }

        public void AddJob(JobPool jobPool)
        {
            jobs.Add(jobPool);
            jobPool.OnComplete += () => jobs.Remove(jobPool);
        }

        public bool CanHireWorker()
        {
            foreach(var job in jobs)
            {
                if (job.CanHireWorker()) return true; 
            }
            return false;
        }

        public void HireWorker(Entity worker)
        {
            DebugUtils.Assert(CanHireWorker());

            foreach(var job in jobs)
            {

                // Find the first job that can hire workers and hire the worker
                if (job.CanHireWorker())
                {
                    job.HireWorker(worker);
                    break;
                } 
            }
        }

        public bool IsComplete()
        {
            return jobs.Count == 0;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"Task: {desc}\n");
            for(int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                builder.Append($"{StringUtils.AddIndentation(job, 2)}{(i == jobs.Count - 1 ? "" : "\n")}");
            }

            return builder.ToString();
        }
    }
}
