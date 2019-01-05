using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class EventTable<TBase>
    {
        // @ Hack Once again, C# generics let us down but this time it's even worse.
        // Instead of being able to use Action<T> where T : IStateChange to have generic
        // listeners for different types of events, we have to use object and cast. Lets
        // hope this doesn't cause problems.
        private Dictionary<Type, List<Action<TBase>>> eventTable;

        // Start is called before the first frame update
        public EventTable()
        {
            eventTable = new Dictionary<Type, List<Action<TBase>>>();
        }

        public void AddListener<T>(Action<T> listener) where T : TBase
        {
            Type t = typeof(T);
            if(!eventTable.ContainsKey(t))
            {
                eventTable.Add(t, new List<Action<TBase>>());
            }

            // Very fancy
            Action<TBase> action = (obj) =>
            {
                var cast = (T)Convert.ChangeType(obj, t);
                listener(cast);
            };
            eventTable[t].Add(action);
        }

        // @Test this needs to be tested, I doubt it works.
        public void RemoveListener<T>(Action<T> listener) where T : TBase
        {
            Type t = typeof(T);
            DebugUtils.Assert(eventTable.ContainsKey(t));

            Action<TBase> action = (obj) =>
            {
                var cast = (T)Convert.ChangeType(obj, t);
                listener(cast);
            };
            eventTable[t].Remove(action);
        }

        public void NotifyListeners<T>(T obj) where T : TBase
        {
            Type t = obj.GetType();
            if (!eventTable.ContainsKey(t)) return;

            foreach(var listener in eventTable[t])
            {
                listener(obj);
            }
        }
    }
}
