using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using Server;
using Client;

namespace Shared
{
    public class SCNetworkManager : MonoBehaviour
    {

        public bool IsHost;
        public GameObject serverPrefab;
        public GameObject clientPrefab;

        [Tooltip("Tick rate of client-server communications.")]
        public static int UPS = 20;
        public static float UPS_INV
        {
            get
            {
                return 1 / (float)UPS;
            }
        }

        void Awake()
        {
            if (IsHost)
            {
                Debug.Log("SCNetworkManager.Awake - initializing server game object.");
                Instantiate(serverPrefab);
            }
            Debug.Log("SCNetworkManager.Awake - initializing client game object.");
            var clientObject = Instantiate(clientPrefab);
            Game.Instance.Client = clientObject.GetComponent<SCClient>(); 
        }

        // Update is called once per frame
        void Update()
        {
        }
    }

}