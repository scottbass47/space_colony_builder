using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private ArrayList selectedTilesPos;

        private Vector3Int startPoint;
        private Vector3Int endPoint;
        private bool dragging;

        public GameObject window;

        private void Start()
        {
            selectedItems = new ArrayList();
            selectedTilesPos = new ArrayList();

            popUpWindow = Instantiate(popUpWindowPrefab).gameObject;
            popUpWindow.SetActive(false);
        }

        private void Update()
        {
            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

            else 
            {
                /* Mouse over pop up window functionality
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

                    Selectable selectable = null;
                    if (obj != null) selectable = obj.GetComponent<Selectable>();

                    //Ctrl click multiple object selection
                    if (Input.GetKey(KeyCode.LeftControl) && obj != null && !hit && selectedItems.Count >= 1 && selectable != null)
                    {
                        if (!selectedItems.Contains(obj))
                        {
                            selectable.enabled = true;
                            selectedItems.Add(obj);
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                        }
                        Selectable.DisplayWindow(window, selectedItems);
                    }
                    //Single object selection
                    else if (obj != null && !hit && selectable != null)
                    {
                        selectable.DisplayWindow(window);
                        if (selectedItems.Count >= 1)
                        {
                            foreach (GameObject item in selectedItems)
                                if (item != null)
                                {
                                    item.GetComponent<Selectable>().enabled = false;
                                    item.transform.GetChild(0).gameObject.SetActive(false);
                                }

                            selectedItems.Clear();
                        }
                        if (!selectedItems.Contains(obj))
                        {
                            selectable.enabled = true;
                            selectedItems.Add(obj);
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    //Clicking to cancel current selection
                    else if (!hit && !Input.GetKey(KeyCode.LeftControl))
                    {
                        foreach (GameObject item in selectedItems)
                            if (item != null)
                            {
                                item.GetComponent<Selectable>().enabled = false;
                                item.transform.GetChild(0).gameObject.SetActive(false);
                            }

                        selectedItems.Clear();
                        if(window.activeSelf) Selectable.DisplayWindow(window, selectedItems);
                    }

                    Camera.main.transform.position = old;

                    startPoint = tilePos;
                    endPoint = tilePos;
                    dragging = true;
                }

                //Rect drag multiple object selection
                else if (Input.GetMouseButton(0) && dragging)
                {
                    var old = Camera.main.transform.position;
                    Camera.main.transform.position = old + new Vector3(0, 0, 11);

                    Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                    Vector3Int tilePos = new Vector3Int(mousePos.x, mousePos.y, 1);

                    Camera.main.transform.position = old;

                    if (endPoint != tilePos)
                    {
                        //Reset selection to adjust for new selected rectangle
                        foreach (GameObject item in selectedItems)
                            if (item != null)
                            {
                                item.GetComponent<Selectable>().enabled = false;
                                item.transform.GetChild(0).gameObject.SetActive(false);                              
                            }
                        selectedItems.Clear();
                        foreach (Vector3Int oldPos in selectedTilesPos) tilemap.SetColor(oldPos, new Color(1, 1, 1));

                        //Iterate through each selected tile and color the tile/select any objects in rectangle
                        endPoint = tilePos;
                        endPoint = endPoint - startPoint;
                        for(int x = 0; x <= Math.Abs(endPoint.x); x++)                      
                            for (int y = 0; y <= Math.Abs(endPoint.y); y++)
                            {
                                var pos = new Vector3Int(x*Math.Sign(endPoint.x), y*Math.Sign(endPoint.y), 0);
                                pos.x += startPoint.x;
                                pos.y += startPoint.y;
                                GameObject obj = Game.Instance.World.GetMapObject(pos);
                                tilemap.SetColor(pos, new Color(0, .50f, 1));
                                Selectable selectable = null;
                                
                                if (obj != null) selectable = obj.GetComponent<Selectable>();
                                if (obj != null && selectable != null)
                                {
                                    if (!selectedItems.Contains(obj))
                                    {
                                        selectable.enabled = true;
                                        selectedItems.Add(obj);
                                        obj.transform.GetChild(0).gameObject.SetActive(true);
                                    }
                                }
                                selectedTilesPos.Add(pos);
                            }
                        if(selectedItems.Count >= 1) Selectable.DisplayWindow(window, selectedItems); 
                    }

                }
                else
                {
                    dragging = false;
                    foreach (Vector3Int oldPos in selectedTilesPos) tilemap.SetColor(oldPos, new Color(1, 1, 1));

                }
            }
        }




    }
}