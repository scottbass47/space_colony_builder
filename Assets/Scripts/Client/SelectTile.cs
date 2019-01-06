using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    public class SelectTile : MonoBehaviour
    {
        public Transform windowPrefab;

        private Tilemap tilemap;
        private GameObject popUpWindow;

        public GameObject window;

        private void Start()
        {
            popUpWindow = Instantiate(windowPrefab).gameObject;
            popUpWindow.SetActive(false);
            tilemap = null;
        }

        private void Update()
        {
            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

            else 
            {
                //Raycast hit checks whether or not user is clicking on UI window
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                var old = Camera.main.transform.position;
                Camera.main.transform.position = old + new Vector3(0, 0, 11);

                Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));

                GameObject obj = Game.Instance.World.GetMapObject(new Vector3Int(mousePos.x, mousePos.y, 0));

                if (popUpWindow.activeSelf && !hit) popUpWindow.SetActive(false);

                if (obj != null && !hit)
                {
                    var selectable = obj.GetComponent<Selectable>();
                    if(selectable != null) selectable.DisplayPopUpWindow(popUpWindow);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (obj != null && !hit)
                    {
                        var selectable = obj.GetComponent<Selectable>();
                        if (selectable != null) selectable.DisplayWindow(window);
                    }
                    else if (!hit) window.SetActive(false);
                    
                }
                Camera.main.transform.position = old;
            }
        }
    }
}