using System;
using System.Collections;
using System.Collections.Generic;
using ECS;

namespace Server.Job
{
    public abstract class IJob 
    {
        private List<Func<bool>> terminationConditions;
        public List<Func<bool>> TerminationConditions => terminationConditions;
        public bool RemoveWorkerFromPool { get; set; } = true;

        public IJob()
        {
            terminationConditions = new List<Func<bool>>();
        }

        protected void AddTerminationCondition(Func<bool> terminationCondition)
        {
            terminationConditions.Add(terminationCondition);
        }

        public bool IsFinished()
        {
            foreach(var term in terminationConditions)
            {
                if (term()) return true;
            }
            return false;
        }

        // Called before all other methods
        public abstract void SetEntity(Entity entity);

        // Called once after SetEntity but before Update
        public abstract void Init();

        // Called once after first frame that IsFinished is true
        public abstract void Done();

        public abstract void OnUpdate(float delta);

    }
}