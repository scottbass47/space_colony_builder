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
    [RequireComponent(typeof(NetObject))]
    public class EntityObject : MonoBehaviour
    {
        public int ID;

        [HideInInspector]
        public EntityType Type => GetComponent<NetObject>().NetObj.EntityType;

        private EventTable<NetUpdate> listeners = new EventTable<NetUpdate>();

        private void Awake()
        {
            GetComponent<NetObject>().RegisterChild(NetObjectType.COMPONENT, OnComponentCreate, OnComponentUpdate, OnComponentDestroy);
        }

        private void OnComponentUpdate(INetObject netObj, NetUpdate update)
        {
            listeners.NotifyListeners(update);
        }

        private void OnComponentCreate(INetObject obj)
        {
        }

        private void OnComponentDestroy(INetObject obj)
        {
        }

        //// This should be called ASAP (once the ID has been assigned, but not before)
        //public void OnCreate()
        //{
        //    //Debug.Log($"EntityObject.AddToEntityManager - Adding entity object with ID {ID} to entity manager.");
        //    Game.Instance.EntityManager.AddEntity(ID, this.gameObject);
        //    Game.Instance.StateChangeManager.AddEntityUpdateListener(this);
        //}

        //private void OnDestroy()
        //{
        //    //Debug.Log($"EntityObject.OnDestroy - Removing entity object with ID {ID} from entity manager.");
        //    Game.Instance.EntityManager.RemoveEntity(ID);
        //    Game.Instance.StateChangeManager.RemoveEntityUpdateListener(this);
        //}

        public void AddUpdateListener<T>(Action<T> listener) where T : NetUpdate
        {
            listeners.AddListener(listener);
        }

        public void RemoveUpdateListener<T>(Action<T> listener) where T : NetUpdate
        {
            listeners.RemoveListener(listener);
        }

        public void OnEntityUpdate<T>(T update) where T : NetUpdate
        {
            listeners.NotifyListeners(update);
        }
    }
}