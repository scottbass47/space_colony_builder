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

        private void Start()
        {
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<PositionUpdate>((position) =>
            {
                Position = position.Pos;
                Position.z = 1;
            });
        }

        private void Update()
        {
            var actual = IsoConversion();
            transform.position = actual;
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
