using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class ComponentType
    {
        private static Dictionary<Type, ComponentType> compTypeMap = new Dictionary<Type, ComponentType>();
        private static int globalIndex;

        public int Index { get; private set; }

        private ComponentType()
        {
            Index = globalIndex++;
        }

        public static ComponentType Get(Type compType)  
        {
            if (!Utils.IsComponent(compType)) throw new ArgumentException("Component type must be a subtype of Component.");
            
            ComponentType t = compTypeMap.ContainsKey(compType) ? compTypeMap[compType] : null;
            if(t == null)
            {
                t = new ComponentType();
                compTypeMap[compType] = t;
            }
            return t;
        }

        public static int GetIndex(Type compType)
        {
            if (!Utils.IsComponent(compType))throw new ArgumentException("Component type must be a subtype of Component.");

            return Get(compType).Index;
        }


    }
}
