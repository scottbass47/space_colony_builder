using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public class Group : IEquatable<Group>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Group);
        }

        public bool Equals(Group other)
        {
            return other != null &&
                   EqualityComparer<Bits>.Default.Equals(all, other.all) &&
                   EqualityComparer<Bits>.Default.Equals(one, other.one) &&
                   EqualityComparer<Bits>.Default.Equals(none, other.none) &&
                   EqualityComparer<Func<Entity, bool>>.Default.Equals(CustomFilter, other.CustomFilter);
        }

        public override int GetHashCode()
        {
            var hashCode = -1979714146;
            hashCode = hashCode * -1521134295 + EqualityComparer<Bits>.Default.GetHashCode(all);
            hashCode = hashCode * -1521134295 + EqualityComparer<Bits>.Default.GetHashCode(one);
            hashCode = hashCode * -1521134295 + EqualityComparer<Bits>.Default.GetHashCode(none);
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Entity, bool>>.Default.GetHashCode(CustomFilter);
            return hashCode;
        }

        public static bool operator ==(Group group1, Group group2)
        {
            return EqualityComparer<Group>.Default.Equals(group1, group2);
        }

        public static bool operator !=(Group group1, Group group2)
        {
            return !(group1 == group2);
        }
    }
}
