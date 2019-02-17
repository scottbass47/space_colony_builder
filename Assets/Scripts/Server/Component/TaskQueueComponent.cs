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
        private Entity player;
        public Entity Player
        {
            get => player;
            set
            {
                player = value;
                net.SetSingleClient(player.GetComponent<ClientComponent>().ID);
            }
        } 

        protected override void OnInit(NetObject netObj)
        {
            taskQueue = new TaskQueue(netObj);
        }

        //public void AddTask(Task task)
        //{
        //    taskQueue.AddTask(task);
        //    queueChanged.Value = true;
        //}

        public void ForceUpdate()
        {
            taskQueue.ForceUpdate();
        }
    }
}
