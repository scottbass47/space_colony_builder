using ECS;
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

        private NetValue<bool> queueChanged;

        public TaskQueueComponent()
        {
            taskQueue = new TaskQueue();

            queueChanged = new NetValue<bool>();
            AddNetValue(queueChanged);
        }

        //public void AddTask(Task task)
        //{
        //    taskQueue.AddTask(task);
        //    queueChanged.Value = true;
        //}

        public override SCUpdate CreateChange()
        {
            return new TaskQueueUpdate { QueueText = taskQueue.ToString() };
        }

        public void ForceUpdate()
        {
            queueChanged.Value = true;
        }

        public override void OnResetTemp()
        {
            queueChanged = new NetValue<bool>();
            AddNetValue(queueChanged);
        }
    }
}
