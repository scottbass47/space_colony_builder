using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class SCNetworkManager : MonoBehaviour
{

    public bool IsHost;
    public GameObject serverPrefab;
    public GameObject clientPrefab;

    public static SCClient Client;

    void Awake()
    {
        if(IsHost)
        {
            Debug.Log("SCNetworkManager.Awake - initializing server game object.");
            Instantiate(serverPrefab);
        }
        Debug.Log("SCNetworkManager.Awake - initializing client game object.");
        var clientObject = Instantiate(clientPrefab);
        Client = clientObject.GetComponent<SCClient>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
