using Shared;
using Shared.StateChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    // Updates transform to be converted isometric position
    [RequireComponent(typeof(EntityObject))]
    public class IsometricPosition : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 Position;

        private Vector3 actual;
        private Vector3 direction;
        private void Start()
        {
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<PositionUpdate>((position) =>
            {
                Position = position.Pos;
                Position.z = 1;
            });
            actual = new Vector3();
        }

        private void Update()
        {
            
            actual = IsoConversion();
            transform.position = actual;
             
/*
            if (actual != IsoConversion())
            {
                actual = IsoConversion();
                direction = new Vector3(actual.x - transform.position.x, actual.y - transform.position.y, 1);
            }

            if (direction.magnitude > .5) 
                transform.Translate(direction * Time.deltaTime * .1f);*/
        }

        public Vector3 IsoConversion()
        {
            Vector3 actual = new Vector3();
            actual.x = (Position.x - Position.y) * 0.5f;
            actual.y = (Position.x + Position.y) * 0.25f + Position.z * 0.5f;
            actual.z = Position.z;
            return actual;
        }
    }
}
