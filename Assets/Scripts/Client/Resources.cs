using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(EntityObject))]
    public class Resources : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<OreUpdate>((ore) => 
            {
                Debug.Log($"Total ore: {ore.Amount}");
            });
        }
    }

}