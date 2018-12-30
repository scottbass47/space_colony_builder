using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public sealed class Utils
    {
        public static bool IsComponent(Type type)
        {
            return typeof(Component).IsAssignableFrom(type);
        }
    }
}
