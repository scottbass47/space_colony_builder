using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProperties : MonoBehaviour
{
    [HideInInspector]
    public int Health;

    [HideInInspector]
    public int workers;

    void Start()
    {
        Health = (int)Random.Range(0f, 10f);
        workers = 0;
        var eo = GetComponent<EntityObject>();
        eo.AddUpdateListener<HealthUpdate>(
            (health) =>
            {
                Health = health.Health;    
                //Debug.Log($"RockHealthTest - health for entity {eo.ID} is {health.Health}");
                });
    }

}
