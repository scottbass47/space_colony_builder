using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server
{
    public class OwnedWorkersComponent : Component
    {
        private List<Entity> workers;
        public int MaxWorkers { get; set; }
        public int NumWorkers => workers.Count;

        public OwnedWorkersComponent()
        {
            workers = new List<Entity>();
        }

        public OwnedWorkersComponent Set(int maxWorkers)
        {
            MaxWorkers = maxWorkers;
            return this;
        }

        public bool HasAvailableWorkers()
        {
            foreach(var worker in workers)
            {
                var w = worker.GetComponent<WorkerComponent>();
                if (w.AvailableForHire()) return true;
            }
            return false;
        }

        public Entity NextAvailableWorker()
        {
            DebugUtils.Assert(HasAvailableWorkers(), "No available workers!");
            foreach(var worker in workers)
            {
                var w = worker.GetComponent<WorkerComponent>();
                if (w.AvailableForHire())
                {
                    return worker;
                }
            }
            return null;
        }

        public void AddWorker(Entity worker)
        {
            DebugUtils.Assert(NumWorkers < MaxWorkers, "Can't add any more workers, already at max.");
            workers.Add(worker);
        }

        public override void OnReset()
        {
            workers.Clear();
            MaxWorkers = 0;
        }
    }
}
