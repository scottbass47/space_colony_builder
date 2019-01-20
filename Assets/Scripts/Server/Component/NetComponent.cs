using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared;
using Shared.StateChanges;

namespace Server 
{
    public abstract class NetComponent : Component 
    {
        private bool hasChanges;
        public bool HasChanges => hasChanges;

        public abstract SCUpdate CreateChange();
        public abstract void OnResetTemp();

        //public NetComponent()
        //{
        //    Type t = GetType();

        //    foreach(var field in t.GetFields())
        //    {
        //        if(field.GetType() == typeof(NetValue<>))
        //        {
        //            NetValue<> v = field.GetValue(this) as NetValue<>;
        //        }
        //    }
        //}

        protected void AddNetValue<T>(NetValue<T> val)
        {
            val.OnChange += OnChange;
        }

        private void OnChange<T>(T val)
        {
            hasChanges = true; 
        }

        public void ResetChanges()
        {
            hasChanges = false;
        }

        public override void OnReset()
        {
            hasChanges = false;
            OnReset();
        }
    }
}
