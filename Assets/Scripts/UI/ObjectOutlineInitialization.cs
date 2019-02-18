using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectOutlineInitialization : MonoBehaviour
{

    Tilemap tilemap;
    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        var to = GetComponentInParent<TilemapObject>();
        var outlinePos = tilemap.CellToWorld(new Vector3Int(to.Pos.x, to.Pos.y, 1));
        outlinePos.y += .25f;
        transform.SetPositionAndRotation(outlinePos, Quaternion.identity);
    }

    
    void Update()
    {
        
    }
}
