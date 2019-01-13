using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    public class SelectTile : MonoBehaviour
    {
        public Transform popUpWindowPrefab;
        public Transform windowPrefab;

        private Tilemap tilemap;
        private GameObject popUpWindow;
        private ArrayList selectedItems;
        private ArrayList selectedItemsPos;
        private Color selectedColor;

        public GameObject window;

        private void Start()
        {
            selectedItems = new ArrayList();
            selectedItemsPos = new ArrayList();
            selectedColor = new Color(1f, .40f, 0f, .8f);
            popUpWindow = Instantiate(popUpWindowPrefab).gameObject;
            popUpWindow.SetActive(false);
        
            /*
            window = Instantiate(windowPrefab, FindObjectOfType<Canvas>().transform).gameObject;
            var rt = window.GetComponent<RectTransform>();
            rt.transform.position = new Vector3(650, 30, 0);
            */

            tilemap = null;
        }

        private void Update()
        {
            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

            else 
            {
                /*
                if (popUpWindow.activeSelf && !hit) popUpWindow.SetActive(false);

                if (obj != null && !hit)
                {
                    var selectable = obj.GetComponent<Selectable>();
                    if(selectable != null) selectable.DisplayPopUpWindow(popUpWindow);
                }*/

                if (Input.GetMouseButtonDown(0))
                {
                    //Test for ui hit
                    bool hit = UIRaycastCheck.CheckUIHit();

                    var old = Camera.main.transform.position;
                    Camera.main.transform.position = old + new Vector3(0, 0, 11);

                    Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                    Vector3Int tilePos = new Vector3Int(mousePos.x, mousePos.y, 1);

                    GameObject obj = Game.Instance.World.GetMapObject(new Vector3Int(mousePos.x, mousePos.y, 0));

                    if (Input.GetKey(KeyCode.LeftControl) && obj != null && !hit && selectedItems.Count >= 1)
                    {
                        if (!selectedItems.Contains(obj))
                        {
                            selectedItems.Add(obj);
                            selectedItemsPos.Add(tilePos);
                            tilemap.SetColor(tilePos, selectedColor);
                        }
                        Selectable.DisplayWindow(window, selectedItems);
                    }
                    else if (obj != null && !hit)
                    {
                        var selectable = obj.GetComponent<Selectable>();
                        if (selectable != null) selectable.DisplayWindow(window);
                        if (selectedItems.Count == 1) selectedItems.Clear();
                        if (!selectedItems.Contains(obj))
                        {
                            selectedItems.Add(obj);
                            selectedItemsPos.Add(tilePos);
                            tilemap.SetColor(tilePos, selectedColor);
                        }
                    }
                    else if (!hit)
                    {
                        foreach (Vector3Int pos in selectedItemsPos) tilemap.SetColor(pos, new Color(1, 1, 1, 1));
                        selectedItems.Clear();
                        Selectable.DisplayWindow(window, selectedItems);
                    }

                    Camera.main.transform.position = old;
                }
                
            }
        }
    }
}