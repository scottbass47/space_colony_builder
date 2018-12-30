using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Shared.SCData;

namespace Server
{
    public class WorldGeneration
    {
        public static TileID[][] GenerateWorld(int size, float stonePercentage)
        {
            TileID[][] map = new TileID[size][];

            for (int x = 0; x < size; x++)
            {
                map[x] = new TileID[size];
                for (int y = 0; y < size; y++)
                {
                    map[x][y] = TileID.GROUND;

                    if (Random.Range(0, 100) < stonePercentage)
                        map[x][y] = TileID.TOWER;
                }
            }
            return map;
        }
    }
}