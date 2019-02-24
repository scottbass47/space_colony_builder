using ECS;
using Server.Job;
using Server.NetObjects;
using Shared;
using Shared.SCData;
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
        private NetObject net;
        private bool taskStarted = false;

        public Task(string desc)
        {
            this.desc = desc;
            jobs = new List<JobPool>();
        }

        public void AddJob(JobPool jobPool)
        {
            jobs.Add(jobPool);
            jobPool.OnComplete += () =>
            {
                jobs.Remove(jobPool);
            };
            jobPool.OnInProgress += () =>
            {
                if (!taskStarted)
                {
                    taskStarted = true;
                    net.Sync();
                }
            };
        }

        public void AddToQueue(TaskQueue queue, int index)
        {
            var parentNetObj = queue.TaskQueueNet;
            net = parentNetObj.CreateChild(NetObjectType.TASK, data: new TaskCreateData { Title = desc });
            net.NetMode = NetMode.IMPORTANT;
            net.OnUpdate = () =>
            {
                var status = !taskStarted ? "Incomplete" : "In Progress";
                return new TaskUpdate { Status = status };
            };
            net.Sync();
        }

        public void RemoveFromQueue()
        {
            net.DestroyData = new TaskDestroyData { DummyText = "shiiiit dawg" };
            net.Destroy();
        }

        public bool CanHireWorker()
        {
            for(int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                if (job.CanHireWorker()) return true; 

                // If the job can't hire a worker AND it hasn't been started,
                // then that means the job is no longer available. So we can remove it.
                else if(job.GetStatus() == JobStatus.INCOMPLETE)
                {
                    jobs.RemoveAt(i);
                    i--;
                }
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
            net.Sync();
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
