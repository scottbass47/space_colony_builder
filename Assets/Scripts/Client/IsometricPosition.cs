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
        private Vector3 position;
        public Vector3 Position => position; 

        private Tilemap tilemap;

        private readonly float TILE_WIDTH_HALF = 0.5f;
        private readonly float TILE_HEIGHT_HALF = 0.25f;

        private void Update()
        {
            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

            transform.position = IsoToWorld(Position);
        }

        public void Translate(Vector3 offset)
        {
            position += offset;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 IsoToWorld(Vector3 iso)
        {
            Vector3 world = new Vector3();
            world.x = (iso.x - iso.y) * TILE_WIDTH_HALF;
            world.y = (iso.x + iso.y) * TILE_HEIGHT_HALF + iso.z * 0.5f;
            world.z = iso.z;
            return world;
        }

        public Vector3 WorldToIso(Vector3 world)
        {
            Vector3 iso = new Vector3();
            iso.x = (world.x / TILE_WIDTH_HALF + world.y / TILE_HEIGHT_HALF) / 2;
            iso.y = (world.y / TILE_HEIGHT_HALF - (world.x / TILE_WIDTH_HALF)) / 2;
            iso.z = world.z;
            return iso;
        }
    }
}
