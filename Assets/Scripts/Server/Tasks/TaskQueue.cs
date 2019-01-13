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

        public int Count => queue.Count;
        public bool Empty => Count == 0;

        public TaskQueue()
        {
            queue = new List<Task>();
        }

        public void AddTask(Task task)
        {
            queue.Add(task);
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
