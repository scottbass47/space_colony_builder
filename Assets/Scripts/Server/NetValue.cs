using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NetValue<T>
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                if(OnChange != null) OnChange(value);
                this.value = value;
            }
        }

        public event Action<T> OnChange;

        public NetValue(T value)
        {
            this.value = value;
        }

        public static implicit operator NetValue<T>(T val)
        {
            return new NetValue<T>(val);
        }

        public static implicit operator T(NetValue<T> val)
        {
            return val.Value;
        }
    }
}
