using Shared;
using Shared.SCData;
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
        public EntityType Type;
        private EventTable<SCUpdate> listeners = new EventTable<SCUpdate>();

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

        public void AddUpdateListener<T>(Action<T> listener) where T : SCUpdate
        {
            listeners.AddListener(listener);
        }

        public void RemoveUpdateListener<T>(Action<T> listener) where T : SCUpdate
        {
            listeners.RemoveListener(listener);
        }

        public void OnEntityUpdate<T>(T update) where T : SCUpdate
        {
            listeners.NotifyListeners(update);
        }
    }
}