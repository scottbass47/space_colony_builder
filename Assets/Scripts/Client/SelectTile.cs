using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectTile : MonoBehaviour
{
    public Camera camera;
    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);


            if (hit.collider != null)
             {
                for(int i = 3; i > 0; i--)
                {
                    Vector3 test = new Vector3(hit.point.x, hit.point.y, i);
                    Vector3Int pos = tilemap.WorldToCell(test);
                    if (tilemap.HasTile(pos))
                    {
                        Debug.Log("Tile " + i + ":" + tilemap.GetTile(pos));
                        break;
                    }
                }
               
             }
             

        }
    }
}
