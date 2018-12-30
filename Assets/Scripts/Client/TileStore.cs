using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Shared.SCData;

namespace Client
{
    public class TileStore : MonoBehaviour
    {
        public TileBase[] tiles;

        public TileBase Get(int index)
        {
            return tiles[index];
        }

        // @Dangerous Casting enums to ints 
        public TileBase Get(TileID tileID)
        {
            return tiles[(int)tileID];
        }
    }
}