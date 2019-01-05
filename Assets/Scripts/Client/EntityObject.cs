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
        private EventTable<EntityUpdate> listeners = new EventTable<EntityUpdate>();

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
            listeners.AddListener(listener);
        }

        public void RemoveUpdateListener<T>(Action<T> listener) where T : EntityUpdate
        {
            listeners.RemoveListener(listener);
        }

        public void OnEntityUpdate<T>(T update) where T : EntityUpdate
        {
            listeners.NotifyListeners(update);
        }
    }
}