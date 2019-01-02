using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;

namespace Client
{
    public class Game : MonoBehaviour
    {

        public GameObject NetManagerPrefab;
        public GameObject EntityObjectFactoryPrefab;
        public GameObject EntityManagerPrefab;
        
        [HideInInspector]
        public SCNetworkManager NetManager;

        [HideInInspector]
        public SCClient Client;

        [HideInInspector]
        public EntityObjectFactory EntityObjectFactory;

        [HideInInspector]
        public EntityManager EntityManager;

        [HideInInspector]
        public EntityPrefabTable PrefabTable;

        private static Game instance;

        public static Game Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            NetManager = Instantiate(NetManagerPrefab).GetComponent<SCNetworkManager>();
            EntityManager = Instantiate(EntityManagerPrefab).GetComponent<EntityManager>();
            
            var factory = Instantiate(EntityObjectFactoryPrefab);
            EntityObjectFactory = factory.GetComponent<EntityObjectFactory>();
            PrefabTable = factory.GetComponent<EntityPrefabTable>();
        }

    }

}