using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(EntityObject))]
    public class Resources : MonoBehaviour
    {
        private int ore;

        public int Ore => ore;

        // Start is called before the first frame update
        void Start()
        {
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<OreUpdate>((oreUpdate) => 
            {
                ore = oreUpdate.Amount;
            });
        }
    }

}