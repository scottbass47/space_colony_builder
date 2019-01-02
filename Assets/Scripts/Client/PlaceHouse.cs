using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceHouse : MonoBehaviour
{
    public TileBase house;

    private Tilemap tilemap;
    private Vector3Int prevBuildingPos;

    void Start()
    { 
        tilemap = FindObjectOfType<Tilemap>();
        prevBuildingPos = new Vector3Int(-10, -10, 10);
    }

    void Update()
    {
        if (tilemap.HasTile(prevBuildingPos)) tilemap.SetTile(prevBuildingPos, null);

        var old = Camera.main.transform.position;
        Camera.main.transform.position = old + new Vector3(0, 0, 11);

        Vector3 mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));

        Vector3Int buildingPos = new Vector3Int((int)mousePos.x, (int)mousePos.y, 1);
        tilemap.SetTile(buildingPos, house);
        tilemap.SetColor(buildingPos, new Color(1f, 1f, 1f, 0.3f));

        if (Input.GetMouseButtonDown(0))
        {  
            if (tilemap.HasTile(new Vector3Int((int)mousePos.x, (int)mousePos.y, 0)) && !tilemap.HasTile(buildingPos))
            { 
                tilemap.SetColor(buildingPos, new Color(1f, 1f, 1f, 1f));
                enabled = false;
            }
            else
            {
                tilemap.SetTile(buildingPos, null);
                enabled = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            tilemap.SetTile(buildingPos, null);
            enabled = false;
        }
        Camera.main.transform.position = old;
        prevBuildingPos = buildingPos;
    }
}
