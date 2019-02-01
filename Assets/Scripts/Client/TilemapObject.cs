using Shared;
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

        // Previously, when a TilemapObject is created we add it immediately to the map. However,
        // this causes problems if the position of the object isn't set initially. So instead,
        // we can just wait until we get the first position update before adding the map object
        // to the world.
        private bool addedToMap;

        public event Action<GameObject, Vector3Int, Vector3Int> PosChanged;

        private void Awake()
        {
            addedToMap = false;

            GetComponent<EntityObject>().AddUpdateListener<PositionUpdate>((pos) =>
            {
                var p = pos.Pos;
                Pos = new Vector3Int((int)p.x, (int)p.y, (int)p.z);
                if (!addedToMap)
                {
                    Game.Instance.World.AddMapObject(this.gameObject);
                    addedToMap = true;
                }
            });
        }

        //private void Start()
        //{
        //    //Game.Instance.World.Ground.GetComponent<MapObjectRenderer>().AddMapObject(this.gameObject);       
        //    Game.Instance.World.AddMapObject(this.gameObject);
        //}

        private void OnDestroy()
        {
            //Game.Instance.World.Ground.GetComponent<MapObjectRenderer>().RemoveMapObject(this.gameObject);
            Game.Instance.World.RemoveMapObject(this.gameObject);
        }
    }

}