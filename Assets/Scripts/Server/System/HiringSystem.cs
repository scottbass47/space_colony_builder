using ECS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class HiringSystem : AbstractSystem
    {
        private List<Entity> workerPool;

        public HiringSystem(List<Entity> workerPool) : base(Group.createGroup().All(typeof(HiringComponent)))
        {
            this.workerPool = workerPool;
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var hire = entity.GetComponent<HiringComponent>();

                var randomWorker = workerPool[Random.Range(0, workerPool.Count)];
                var worker = randomWorker.GetComponent<WorkerComponent>();
                worker.AssignJob(hire.OnHire(randomWorker), randomWorker);
                workerPool.Remove(randomWorker);
                entity.RemoveComponent<HiringComponent>();
            }
        }
    }
}
