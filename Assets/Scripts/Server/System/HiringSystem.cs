using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Server
{
    public class HiringSystem : AbstractSystem
    {
        private List<Entity> owners;

        public HiringSystem() : base(Group.createGroup().All(typeof(NotOwnedComponent)))
        {
            owners = new List<Entity>();
        }

        public override void AddedToEngine()
        {
            Engine.AddGroupListener(Group.createGroup().All(typeof(OwnedWorkersComponent)), AddOwner, RemoveOwner);
        }

        private void AddOwner(Entity obj)
        {
            owners.Add(obj);
        }

        private void RemoveOwner(Entity obj)
        {
            owners.Remove(obj);
        }

        public override void Update(float delta)
        {
            var entities = Entities;
            int idx = 0;

            foreach(var owner in owners)
            {
                var o = owner.GetComponent<OwnedWorkersComponent>();

                // If we can hire workers, start hiring
                if(o.NumWorkers < o.MaxWorkers)
                {
                    for(; idx < entities.Count; idx++)
                    {
                        var worker = entities[idx];
                        o.AddWorker(worker);
                        worker.RemoveComponent<NotOwnedComponent>();
                        if (o.NumWorkers == o.MaxWorkers) break;
                    }
                    
                }
            }
        }
    }
}
