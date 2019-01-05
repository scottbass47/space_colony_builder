using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Operation : Poolable
    {
        public OpType type;
        public Entity entity;

        public Operation Set(OpType type, Entity entity)
        {
            this.type = type;
            this.entity = entity;

            return this;
        }

        public void Reset()
        {
            entity = null;
        }
    }

    public enum OpType
    {
        Add,
        Remove,
        UpdateGroup
    }
}