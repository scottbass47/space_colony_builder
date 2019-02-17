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
    public class TaskQueue
    {
        private List<Task> queue;
        private NetObject net;

        public int Count => queue.Count;
        public bool Empty => Count == 0;
        public NetObject TaskQueueNet => net;

        public TaskQueue(NetObject parentNetObj)
        {
            queue = new List<Task>();
            net = parentNetObj.CreateChild(netObjectType: NetObjectType.TASK_QUEUE);
            net.OnUpdate = () => new TaskQueueUpdate { QueueText = ToString() };
            net.NetMode = NetMode.IMPORTANT;
        }

        public void ForceUpdate()
        {
            //net.Sync();
        }

        public void AddTask(Task task)
        {
            queue.Add(task);
            task.AddToQueue(this, queue.Count - 1);
        }

        public Task Peek()
        {
            DebugUtils.Assert(!Empty, "Can't peek from an empty queue.");
            return queue[0];
        }

        public Task RemoveFirst()
        {
            DebugUtils.Assert(!Empty, "Can't remove from an empty queue.");
            Task t = queue[0];
            t.RemoveFromQueue();
            queue.RemoveAt(0);
            return t;
        }

        public Task this[int index]
        {
            get => queue[index];
            set => throw new Exception("Can't modify queue like this.");
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Task Queue:\n========\n");
            foreach(var task in queue)
            {
                builder.Append($"{task}\n");
            }
            return builder.ToString(); 
        }
    }
}
