using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    public class TilemapObject : MonoBehaviour
    {
        public TileBase Tile;

        private Vector3Int pos = Vector3Int.zero;
        public Vector3Int Pos
        {
            set
            {
                var oldPos = pos;
                var newPos = value;
                if(PosChanged != null)
                {
                    PosChanged(gameObject, oldPos, newPos);
                }
                pos = newPos;
            }
            get => pos;
        }

        public event Action<GameObject, Vector3Int, Vector3Int> PosChanged;

        private void Start()
        {
            //Game.Instance.World.Ground.GetComponent<MapObjectRenderer>().AddMapObject(this.gameObject);       
            Game.Instance.World.AddMapObject(this.gameObject);
        }

        private void OnDestroy()
        {
            //Game.Instance.World.Ground.GetComponent<MapObjectRenderer>().RemoveMapObject(this.gameObject);
            Game.Instance.World.RemoveMapObject(this.gameObject);
        }
    }

}