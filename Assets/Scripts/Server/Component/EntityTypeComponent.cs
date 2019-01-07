using ECS;
using Shared.SCData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class EntityTypeComponent : Component
    {
        public EntityType Type { get; set; }

        public void Reset()
        {
            Type = EntityType.ROCK;
        }
    }
}
