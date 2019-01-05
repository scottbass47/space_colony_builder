using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SelectTile : MonoBehaviour
{
    public Transform UIPrefab;
    public Tilemap tilemap;

    private GameObject UIPopUp;

    private void Start()
    {
        UIPopUp = Instantiate(UIPrefab).gameObject;
        UIPopUp.SetActive(false);
    }

    private void Update()
    {
        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            /*
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
             }*/



            var old = Camera.main.transform.position;
            Camera.main.transform.position = old + new Vector3(0, 0, 11);

            Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            if (UIPopUp.activeSelf && !hit) UIPopUp.SetActive(false);

            if (tilemap.HasTile(new Vector3Int(mousePos.x, mousePos.y, 1)) && !hit)
            {
                Vector3 worldMousePos = tilemap.CellToWorld(mousePos);

                RectTransform rt = UIPopUp.GetComponent<RectTransform>();

                float widthOffset = rt.rect.width * rt.lossyScale.x / 2 + 0.5f;
                float heightOffset = rt.rect.height * rt.lossyScale.y / 2;

                Vector3 UIPos = new Vector3(worldMousePos.x + widthOffset, worldMousePos.y + heightOffset, 1);
              
                /*change to something like this 
                GameObject obj = Game.EntityManager.Get(mousePos);
                var selectable = obj.GetComponent<Selectable>()
                selectable.DisplayWindow(obj.type);
                */

                UIPopUp.SetActive(true);
                UIPopUp.transform.SetPositionAndRotation(UIPos, Quaternion.identity);
            }
            Camera.main.transform.position = old;
        }
    }
}
