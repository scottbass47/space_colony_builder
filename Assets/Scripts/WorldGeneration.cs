using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase grass;
    public TileBase stone;
    public Camera camera;
    public int size;
    public float stonePercentage;
    
    

    // Start is called before the first frame update
    void Awake()
    {
        RenderMap(GenerateArray(size, stonePercentage), tilemap, grass, stone);

        //Readjusts the camera positioning and scaling so new map is centered and fully in frame.
        camera.transform.SetPositionAndRotation(new Vector3(0, size * size / (4 * size), -10), Quaternion.identity);
        camera.orthographicSize = size / 3;
    }

    static int[,] GenerateArray(int size, float stonePercentage)
    {
        int[,] map = new int[size, size];

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (Random.Range(0, 100) < stonePercentage)
                    map[x, y] = 1;

            }
        }

        return map;
    }

    static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile, TileBase tile2)
    {
        tilemap.ClearAllTiles();

        for(int x = 0; x < map.GetUpperBound(0); x++)
        {
            for(int y = 0; y < map.GetUpperBound(1); y++)
            {
                if(map[x,y].Equals(0))
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                if (map[x,y].Equals(1))
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    tilemap.SetTile(new Vector3Int(x, y, 1), tile2);
                }
            }
        }

    }

}
