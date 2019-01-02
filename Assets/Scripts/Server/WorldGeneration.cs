using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Shared.SCData;

namespace Server
{
    public class WorldGeneration
    {
        public static TileID[][] GenerateWorld(int size, float stonePercentage, out List<Vector3Int> rockSpawns)
        {
            TileID[][] map = new TileID[size][];
            rockSpawns = new List<Vector3Int>();

            for (int x = 0; x < size; x++)
            {
                map[x] = new TileID[size];
                for (int y = 0; y < size; y++)
                {
                    map[x][y] = TileID.GROUND;

                    if (Random.Range(0, 100) < stonePercentage)
                        rockSpawns.Add(new Vector3Int { x = x, y = y, z = 0 });
                }
            }
            return map;
        }
    }
}