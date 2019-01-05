using Shared.StateChanges;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class RockHealthTest : MonoBehaviour
    {
        void Start()
        {
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<HealthUpdate>(
                (health) => 
                {
                    //Debug.Log($"RockHealthTest - health for entity {eo.ID} is {health.Health}");
                });
        }

    }

}