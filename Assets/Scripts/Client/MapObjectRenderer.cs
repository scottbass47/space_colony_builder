using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Client
{
    public class MapObjectRenderer : MonoBehaviour
    {
        private List<GameObject> gameObjects;
        private Tilemap tilemap;
        private readonly Vector3Int shift = new Vector3Int(0, 0, 1);

        void Awake()
        {
            gameObjects = new List<GameObject>();
            tilemap = GetComponentInChildren<Tilemap>();
        }

        public void AddMapObject(GameObject go)
        {
            var comp = go.GetComponent<TilemapObject>();
            DrawTileAt(comp.Pos, comp.Tile);
            comp.PosChanged += UpdatePosition;
        }

        public void RemoveMapObject(GameObject go)
        {
            var comp = go.GetComponent<TilemapObject>();
            DrawTileAt(comp.Pos, null);
            comp.PosChanged -= UpdatePosition;
        }

        public void UpdatePosition(GameObject go, Vector3Int oldPos, Vector3Int newPos)
        {
            TileBase tile = go.GetComponent<TilemapObject>().Tile;
            DrawTileAt(oldPos, null);
            DrawTileAt(newPos, tile);
        }

        private void DrawTileAt(Vector3Int pos, TileBase tile)
        {
            // Z = 0 is the ground for tilemap objects, but because we're
            // sharing a tilemap with with the static layer, we need to shift
            // the Z value one up.
            Vector3Int shiftedPos = pos + shift;

            // Overlapping tiles don't make sense. If you try and draw a tile at the
            // same location as an existing tile that's probably an error.
            TileBase oldTile = tilemap.GetTile(shiftedPos);
            DebugUtils.Assert(oldTile == null || tile == null);                        

            tilemap.SetTile(shiftedPos, tile);
        }

    }

}