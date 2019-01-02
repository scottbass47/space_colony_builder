using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class EntityObject : MonoBehaviour
    {
        public int ID;

        // This should be called ASAP (once the ID has been assigned, but not before)
        public void AddToEntityManager()
        {
            //Debug.Log($"EntityObject.AddToEntityManager - Adding entity object with ID {ID} to entity manager.");
            Game.Instance.EntityManager.AddEntity(ID, this.gameObject);
        }

        private void OnDestroy()
        {
            //Debug.Log($"EntityObject.OnDestroy - Removing entity object with ID {ID} from entity manager.");
            Game.Instance.EntityManager.RemoveEntity(ID);
        }
    }
}