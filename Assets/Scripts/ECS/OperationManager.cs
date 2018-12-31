using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public partial class Engine
    {
        private Queue<Operation> pendingOps;

        public Operation CreateOperation()
        {
            return operationPool.Obtain<Operation>();
        }

        public void RecycleOperation(Operation op)
        {
            operationPool.Recycle<Operation>(op);
        }

        public bool hasOps()
        {
            return pendingOps.Count > 0;
        }

        public void ProcessOps()
        {
            foreach(Operation op in pendingOps)
            {
                switch (op.type)
                {
                    case OpType.Add: AddEntity(op.entity); break;
                    case OpType.Remove: RemoveEntity(op.entity); break;
                    case OpType.UpdateGroup: UpdateGroupMembership(op.entity); break;
                    default:
                        throw new System.Exception("Unexpected Op Type");
                }
                RecycleOperation(op);
            }
            
            pendingOps.Clear();
        }
    }
}
