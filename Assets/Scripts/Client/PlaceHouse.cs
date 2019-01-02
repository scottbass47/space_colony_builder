using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceHouse : MonoBehaviour
{
    public TileBase houseTile;
    public Sprite houseSprite;
    public Transform ghostPrefab;

    private Tilemap tilemap;
    private GameObject ghost;
    void Start()
    {
        ghost = Instantiate(ghostPrefab).gameObject;
        ghost.SetActive(false);
        tilemap = FindObjectOfType<Tilemap>();
        ghost.GetComponent<SpriteRenderer>().sprite = houseSprite;
    }

    void Update()
    {
        ghost.SetActive(true);
        var old = Camera.main.transform.position;
        Camera.main.transform.position = old + new Vector3(0, 0, 11);

        Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        Vector3Int buildingPos = new Vector3Int(mousePos.x, mousePos.y, 1);

        Vector3 ghostPos = tilemap.CellToWorld(buildingPos);

        ghostPos.y += .25f;

        ghost.transform.SetPositionAndRotation(ghostPos, Quaternion.identity);

        if (tilemap.HasTile(buildingPos))
        {
            ghost.SetActive(false);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (tilemap.HasTile(new Vector3Int(mousePos.x, mousePos.y, 0)))
                {
                    tilemap.SetTile(buildingPos, houseTile);
                    ghost.SetActive(false);
                    enabled = false;
                }
                else
                {
                    ghost.SetActive(false);
                    enabled = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                ghost.SetActive(false);
                enabled = false;
            }
        }
        Camera.main.transform.position = old;
        
    }
}
