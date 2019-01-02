using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceHouse : MonoBehaviour
{
    public TileBase house;

    private Tilemap tilemap;

    void Start()
    { 
        tilemap = FindObjectOfType<Tilemap>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var old = Camera.main.transform.position;
            Camera.main.transform.position = old + new Vector3(0, 0, 11);
            Vector3 mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            Debug.Log(mousePos);
            Vector3Int buildingPos = new Vector3Int((int)mousePos.x, (int)mousePos.y, 2);
            tilemap.SetTile(buildingPos, house);
            Camera.main.transform.position = old;
        }
        if (Input.GetMouseButtonDown(1)) enabled = false; ;
    }
}
