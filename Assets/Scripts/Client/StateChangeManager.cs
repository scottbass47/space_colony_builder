using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.SCPacket;
using Shared.StateChanges;
using Utils;

namespace Client
{
    public class StateChangeManager : MonoBehaviour
    {
        private int MyVersion = 0;
        private Dictionary<int, List<StateChangePacket>> stateChangeBuffer;

        // @ Hack Once again, C# generics let us down but this time it's even worse.
        // Instead of being able to use Action<T> where T : IStateChange to have generic
        // listeners for different types of events, we have to use object and cast. Lets
        // hope this doesn't cause problems.
        private Dictionary<Type, List<Action<IStateChange>>> eventTable;
        private Dictionary<int, EntityObject> entityUpdateTable;

        // Start is called before the first frame update
        private void Start()
        {
            stateChangeBuffer = new Dictionary<int, List<StateChangePacket>>();
            entityUpdateTable = new Dictionary<int, EntityObject>();
            eventTable = new Dictionary<Type, List<Action<IStateChange>>>();

            AddStateChangeListener<EntitySpawn>((spawn) =>
            {
                var go = Game.Instance.EntityObjectFactory.CreateEntityObject(spawn.EntityType, spawn);
                //Instantiate(go);
            });
            
            AddStateChangeListener<EntityRemove>((remove) =>
            {
                var go = Game.Instance.EntityManager.GetEntity(remove.ID);
                Destroy(go); 
            });

            Game.Instance.Client.AddPacketListener<StateChangePacket>(StateChanges);
        }

        public void AddStateChangeListener<T>(Action<T> listener) where T : IStateChange
        {
            Type t = typeof(T);
            if(!eventTable.ContainsKey(t))
            {
                eventTable.Add(t, new List<Action<IStateChange>>());
            }

            // Very fancy
            Action<IStateChange> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            eventTable[t].Add(action);
        }

        // @Test this needs to be tested, I doubt it works.
        public void RemoveStateChangeListener<T>(Action<T> listener) where T : IStateChange
        {
            Type t = typeof(T);
            DebugUtils.Assert(eventTable.ContainsKey(t));

            Action<IStateChange> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            eventTable[t].Remove(action);
        }

        private void NotifyStateChange<T>(T change) where T : IStateChange
        {
            Type t = change.GetType();
            if(change is EntityUpdate)
            {
                NotifyEntityUpdate(change as EntityUpdate);
            }
            if (!eventTable.ContainsKey(t)) return;

            foreach(var listener in eventTable[t])
            {
                listener(change);
            }
        }

        private void NotifyEntityUpdate<T>(T update) where T : EntityUpdate
        {
            if (!entityUpdateTable.ContainsKey(update.ID)) return;

            entityUpdateTable[update.ID].OnEntityUpdate<T>(update);
        }

        public void AddEntityUpdateListener(EntityObject eo)
        {
            entityUpdateTable.Add(eo.ID, eo);
        }
        
        public void RemoveEntityUpdateListener(EntityObject eo)
        {
            entityUpdateTable.Remove(eo.ID);
        }

        public void StateChanges(StateChangePacket packet)
        {
            if(!(packet.Change is NoChange))
            {
                Debug.Log($"[Client] received {packet.Change.GetType()} change for version {packet.Version}. Change {packet.ChangeNumber}/{packet.TotalChanges}");
            }

            if (!stateChangeBuffer.ContainsKey(packet.Version))
            {
                stateChangeBuffer[packet.Version] = new List<StateChangePacket>();
            }

            stateChangeBuffer[packet.Version].Add(packet);

            // Try and apply changes to the oldest version we don't have
            TryApplyChanges(MyVersion + 1); 
        }

        // Checks if all the changes for a given version number have arrived,
        // and if so, applies the changes and increments the version.
        private void TryApplyChanges(int version)
        {
            if (!stateChangeBuffer.ContainsKey(version)) return;

            if (stateChangeBuffer[version].Count == stateChangeBuffer[version][0].TotalChanges)
            {
                if(!(stateChangeBuffer[version][0].Change is NoChange))
                {
                    Debug.Log($"[Client] - applying changes for version {version}");
                }
                foreach (var packet in stateChangeBuffer[version])
                {
                    IStateChange change = packet.Change;
                    NotifyStateChange(change);
                }

                // After applying all the changes, we can safely increment our version of
                // the world and try to apply the next version's changes.
                MyVersion = version;
                stateChangeBuffer.Remove(version);
                TryApplyChanges(version + 1);
            }
        }

    }

}