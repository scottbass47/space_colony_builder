using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.Job
{
    public abstract class JobPool
    {
        //private int maxWorkers;
        private Dictionary<Entity, IJob> workers;

        //public int MaxWorkers => maxWorkers;
        public int NumWorkers => workers.Count;
        private bool complete = false;

        public event Action OnComplete;

        public JobPool()
        {
            workers = new Dictionary<Entity, IJob>();
        }

        public void HireWorker(Entity worker)
        {
            var job = GetJob(worker);
            worker.GetComponent<WorkerComponent>().AssignJob(job, worker, OnJobComplete);
            workers.Add(worker, job); 
        }

        private void OnJobComplete(Entity worker)
        {
            if(!complete)
            {
                workers.Clear();
                complete = true;
                if (OnComplete != null) OnComplete();
            }
        }

        public JobStatus GetStatus()
        {
            if (complete) return JobStatus.COMPLETE;
            if (NumWorkers == 0) return JobStatus.INCOMPLETE;
            return JobStatus.IN_PROGRESS;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var status = GetStatus();
            builder.Append($"{GetDescription()} ({status}){(status == JobStatus.IN_PROGRESS ? "\n" : "")}");

            int idx = 0;
            foreach(var worker in workers.Keys)
            {
                idx++;
                builder.Append($"   Worker {worker.ID}{(idx == workers.Count ? "" : "\n")}");
            }
            return builder.ToString(); 
        }

        public abstract IJob GetJob(Entity worker);
        public abstract string GetDescription();
        public abstract bool CanHireWorker();
    }

    public enum JobStatus
    {
        INCOMPLETE,
        IN_PROGRESS,
        COMPLETE
    }
}
