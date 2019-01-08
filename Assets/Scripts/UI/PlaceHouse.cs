using Shared;
using Shared.SCPacket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    public class PlaceHouse : MonoBehaviour
    {
        public TileBase houseTile;
        public Sprite houseSprite;
        public Transform ghostPrefab;

        private Tilemap tilemap;
        private GameObject ghost;
        private SelectTile selectTile;

        void Start()
        {
            ghost = Instantiate(ghostPrefab).gameObject;
            ghost.SetActive(false);
            tilemap = FindObjectOfType<Tilemap>();
            ghost.GetComponent<SpriteRenderer>().sprite = houseSprite;

            selectTile = Game.Instance.GetComponent<SelectTile>();
        }

        void Update()
        {
            selectTile.enabled = false;

            ghost.SetActive(true);
            var old = Camera.main.transform.position;
            Camera.main.transform.position = old + new Vector3(0, 0, 11);

            Vector3Int mousePos = tilemap.WorldToCell((Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            Vector3Int buildingPos = new Vector3Int(mousePos.x, mousePos.y, 0);
            Vector3Int ghostPosCell = new Vector3Int(mousePos.x, mousePos.y, 1);

            Vector3 ghostPos = tilemap.CellToWorld(ghostPosCell);

            ghostPos.y += .25f;

            ghost.transform.SetPositionAndRotation(ghostPos, Quaternion.identity);

            if (tilemap.HasTile(ghostPosCell))
            {
                ghost.SetActive(false);
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (tilemap.HasTile(new Vector3Int(mousePos.x, mousePos.y, 0)))
                    {
                        //tilemap.SetTile(buildingPos, houseTile);

                        var client = Game.Instance.Client;
                        client.SendPacket((ClientRequest)(new PlaceBuildingRequest { Pos = buildingPos }));

                        ghost.SetActive(false);
                        selectTile.enabled = true;
                        enabled = false;
                    }
                    else
                    {
                        ghost.SetActive(false);
                        selectTile.enabled = true;
                        enabled = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ghost.SetActive(false);
                    selectTile.enabled = true;
                    enabled = false;
                }
            }
            Camera.main.transform.position = old;

        }
    }
}
