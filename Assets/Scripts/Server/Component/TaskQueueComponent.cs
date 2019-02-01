using ECS;
using Server.NetObjects;
using Server.Tasks;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = Server.Tasks.Task;

namespace Server
{
    public class TaskQueueComponent : NetComponent 
    {
        public TaskQueue Tasks => taskQueue;
        private TaskQueue taskQueue;
        public Entity Player { get; set; }

        private bool qc;
        private bool queueChanged
        {
            get => qc;
            set
            {
                qc = value;
                net.Sync();
            }
        }

        public TaskQueueComponent()
        {
            taskQueue = new TaskQueue();
        }

        protected override void OnInit(NetObject netObj)
        {
            netObj.NetMode = NetMode.IMPORTANT;
            netObj.OnUpdate = () => new TaskQueueUpdate { QueueText = taskQueue.ToString() };
        }

        //public void AddTask(Task task)
        //{
        //    taskQueue.AddTask(task);
        //    queueChanged.Value = true;
        //}

        public void ForceUpdate()
        {
            queueChanged= true;
        }
    }
}
