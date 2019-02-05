using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Shared.SCData;

namespace Server
{
    public class WorldGeneration
    {
        public static TileID[][] GenerateWorld(int size, float orePercentage, out List<Vector3Int> oreSpawns, 
            out Vector3Int landingPad, out Vector3Int house)
        {
            landingPad = new Vector3Int { x = size / 2, y = size / 2, z = 0 };
            house = landingPad + new Vector3Int { x = 1, y = 1, z = 0 };

            TileID[][] map = new TileID[size][];
            oreSpawns = new List<Vector3Int>();

            for (int x = 0; x < size; x++)
            {
                map[x] = new TileID[size];
                for (int y = 0; y < size; y++)
                {
                    map[x][y] = TileID.GROUND;

                    if (Random.Range(0, 100) < orePercentage)
                    {
                        Vector3Int spawn = new Vector3Int { x = x, y = y, z = 0 };
                        if (spawn == landingPad || spawn == house) continue;
                        oreSpawns.Add(spawn);
                    }
                }
            }
            return map;
        }
    }
}