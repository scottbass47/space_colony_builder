using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class TaskProcessingSystem : AbstractSystem
    {
        public TaskProcessingSystem() : base(Group.createGroup().All(typeof(TaskQueueComponent), typeof(OwnedWorkersComponent)))
        {
        }

        private float elapsed;

        public override void Update(float delta)
        {
            elapsed += delta;
            foreach(var entity in Entities)
            {
                var taskQueueComp = entity.GetComponent<TaskQueueComponent>();
                var taskQueue = taskQueueComp.Tasks;

                // While taskQueue.Peek() is complete, dequeue it
                while(!taskQueue.Empty && taskQueue.Peek().IsComplete())
                {
                    taskQueue.RemoveFirst();
                }

                var ownedWorkers = entity.GetComponent<OwnedWorkersComponent>();

                // While taskQueue.Peek() has open slots and 
                // there are available workers, add workers to task

                int taskIdx = 0;
                while(taskIdx < taskQueue.Count)
                {
                    var task = taskQueue[taskIdx];
                    bool noMoreWorkers = false;
                    while(task.CanHireWorker() && ownedWorkers.HasAvailableWorkers())
                    {
                        //task.RemoveCompletedJobs();
                        var worker = ownedWorkers.NextAvailableWorker();
                        task.HireWorker(worker);
                        if (!ownedWorkers.HasAvailableWorkers()) noMoreWorkers = true;
                    }
                    if (noMoreWorkers) break;
                    taskIdx++;
                }
                if(elapsed >= 1f)
                {
                    //Debug.Log(taskQueue.ToString());
                    taskQueueComp.ForceUpdate();
                    elapsed = 0;
                }
            }
        }
    }
}
