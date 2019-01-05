using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.StateChanges;

namespace Server 
{
    public abstract class NetComponent : Component 
    {
        private bool hasChanges;
        public bool HasChanges => hasChanges;

        public abstract EntityUpdate CreateChange();

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
    }
}
