using Shared.StateChanges;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Client
{
    public class EntityObject : MonoBehaviour
    {
        public int ID;
        private Dictionary<Type, List<Action<EntityUpdate>>> listeners = new Dictionary<Type, List<Action<EntityUpdate>>>();

        // This should be called ASAP (once the ID has been assigned, but not before)
        public void OnCreate()
        {
            //Debug.Log($"EntityObject.AddToEntityManager - Adding entity object with ID {ID} to entity manager.");
            Game.Instance.EntityManager.AddEntity(ID, this.gameObject);
            Game.Instance.StateChangeManager.AddEntityUpdateListener(this);
        }

        private void OnDestroy()
        {
            //Debug.Log($"EntityObject.OnDestroy - Removing entity object with ID {ID} from entity manager.");
            Game.Instance.EntityManager.RemoveEntity(ID);
            Game.Instance.StateChangeManager.RemoveEntityUpdateListener(this);
        }

        public void AddUpdateListener<T>(Action<T> listener) where T : EntityUpdate
        {
            Type t = typeof(T);
            if(!listeners.ContainsKey(t))
            {
                listeners.Add(t, new List<Action<EntityUpdate>>());
            }

            // Very fancy
            Action<EntityUpdate> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            listeners[t].Add(action);
        }

        // @Test this needs to be tested, I doubt it works.
        public void RemoveUpdateListener<T>(Action<T> listener) where T : IStateChange
        {
            Type t = typeof(T);
            DebugUtils.Assert(listeners.ContainsKey(t));

            Action<IStateChange> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            listeners[t].Remove(action);
        }

        public void OnEntityUpdate<T>(T update) where T : EntityUpdate
        {
            Type t = update.GetType();
            if (!listeners.ContainsKey(t)) return;
            foreach(var listener in listeners[t])
            {
                listener(update);
            }
        }
    }
}