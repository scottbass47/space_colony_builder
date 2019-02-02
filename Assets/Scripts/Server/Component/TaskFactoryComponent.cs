using ECS;
using Server.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class TaskFactoryComponent : Component
    {
        public TaskFactory Factory { get; set; }

        public TaskFactoryComponent Set(TaskFactory factory)
        {
            Factory = factory;
            return this;
        }

        public override void OnReset()
        {
        }
    }
}
