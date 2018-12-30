using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Group
    {
        private Bits all;
        private Bits one;
        private Bits none;

        public Func<Entity, bool> CustomFilter { get; set; }

        private Group()
        {
            all = new Bits();
            one = new Bits();
            none = new Bits();
        }

        public Group All(params Type[] types)
        {
            foreach(Type t in types)
            {
                if (!Utils.IsComponent(t)) throw new ArgumentException("Types must be a subtype of Component.");
                all.Set(ComponentType.GetIndex(t), true);
            }
            return this;
        }

        public Group One(params Type[] types)
        {
            foreach(Type t in types)
            {
                if (!Utils.IsComponent(t)) throw new ArgumentException("Types must be a subtype of Component.");
                one.Set(ComponentType.GetIndex(t), true);
            }
            return this;
        }

        public Group None(params Type[] types)
        {
            foreach(Type t in types)
            {
                if (!Utils.IsComponent(t)) throw new ArgumentException("Types must be a subtype of Component.");
                none.Set(ComponentType.GetIndex(t), true);
            }
            return this;
        }

        public static Group createGroup()
        {
            return new Group();
        }

        public bool Matches(Entity entity)
        {
            if(CustomFilter != null)
            {
                return CustomFilter(entity);
            }

            if (!entity.ComponentBits.ContainsAll(all)) return false;
            if (!one.Empty && !entity.ComponentBits.Intersects(one)) return false;
            if (!none.Empty && entity.ComponentBits.Intersects(none)) return false;
            
            return true;
        }
    }
}
