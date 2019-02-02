using Shared;
using Shared.SCData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Client
{
    public class NetObject : MonoBehaviour
    {
        private INetObject netObj;
        private Dictionary<NetObjectType, object[]> registries;

        public INetObject NetObj => netObj;

        private void Awake()
        {
            registries = new Dictionary<NetObjectType, object[]>();
        }

        public void RegisterChild(NetObjectType type, Action<INetObject> onCreateChild, Action<INetObject, NetUpdate> onUpdateChild, Action<INetObject> onDestroyChild)
        {
            registries.Add(type, new object[] { onCreateChild, onUpdateChild, onDestroyChild });
        }

        public void SetNetObject(INetObject obj)
        {
            netObj = obj;
            netObj.GameObject = gameObject;

            // Transfer stored registrations to INetObject
            foreach(var type in registries.Keys)
            {
                var actions = registries[type];
                netObj.RegisterChild(type, 
                    (Action<INetObject>)actions[0], 
                    (Action<INetObject, NetUpdate>)actions[1], 
                    (Action<INetObject>) actions[2]
                );
            }
            registries.Clear();
        }
    }

    public class INetObject
    {
        private NetObjectManager nom;
        private HashSet<NetObjectType> registeredChildren;
        private HashSet<int> children;
        private int netID;
        private int parentID; // This is the DIRECT parent of this NetObject
        private bool hasParent;
        private int parentHandlerID; // This is the parent that has THIS NetObject registered as a child
        private bool hasParentHandler;
        private NetObjectType netObjectType;
        private EntityType entityType;
        private GameObject gameObject;
        private bool isGameObject;
        private EventTable<NetUpdate> updateTable;
        private Action<INetObject> onChildCreate; 
        private Action<INetObject, NetUpdate> onChildUpdate;
        private Action<INetObject> onChildDestroy;

        public Action<INetObject> OnChildCreate => onChildCreate;
        public Action<INetObject, NetUpdate> OnChildUpdate => onChildUpdate;
        public Action<INetObject> OnChildDestroy => onChildDestroy; 

        public int NetID => netID;
        public bool HasParent => hasParent;
        public bool HasParentHandler => hasParentHandler;
        public bool IsParent => children.Count > 0;
        public int ParentID => parentID;
        public int ParentHandlerID => parentHandlerID;
        public NetObjectType NetObjectType => netObjectType;
        public EntityType EntityType => entityType;
        public bool IsEntity => entityType != EntityType.NOTHING;
        public GameObject GameObject { get => gameObject; set => gameObject = value; }
        public bool IsGameObject { get => isGameObject; set => isGameObject = value; }
        public HashSet<int> Children => children;

        public INetObject(NetObjectManager nom, NetObjectType netObjectType, EntityType entityType, int netID)
        {
            this.nom = nom;
            this.netObjectType = netObjectType;
            this.entityType = entityType;
            this.netID = netID;
            registeredChildren = new HashSet<NetObjectType>();
            updateTable = new EventTable<NetUpdate>();
            children = new HashSet<int>();
        }

        public void SetParent(int id)
        {
            parentID = id;
            hasParent = true;
        }

        public void SetParentHandler(int id)
        {
            parentHandlerID = id;
            hasParentHandler = true;
        }

        public void AddChild(int id)
        {
            children.Add(id);
        }

        public void RemoveChild(int id)
        {
            children.Remove(id);
        }

        public void RemoveAllChildren()
        {
            children.Clear();
        }

        public void RegisterChild(NetObjectType type, Action<INetObject> onCreate, Action<INetObject, NetUpdate> onUpdate, Action<INetObject> onDestroy)
        {
            registeredChildren.Add(type);
            onChildCreate = onCreate;
            onChildUpdate = onUpdate;
            onChildDestroy = onDestroy;
        }

        public bool HandlesChild(NetObjectType type)
        {
            return registeredChildren.Contains(type);
        }

        public void AddUpdateListner<T>(Action<T> onUpdate) where T: NetUpdate
        {
            updateTable.AddListener(onUpdate);
        }

        public void RemoveUpdateListner<T>(Action<T> onUpdate) where T: NetUpdate
        {
            updateTable.RemoveListener(onUpdate);
        }

        public void OnUpdate<T>(T update) where T : NetUpdate
        {
            updateTable.NotifyListeners(update);
        }
    }
}