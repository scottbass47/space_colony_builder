using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SCPacket;

public class PrintTiles : MonoBehaviour
{
    private Tilemap tileMap;
    private SCClient client;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var mouseLocalPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseLocalPos);
            var tileMapCell = tileMap.WorldToCell(mouseWorldPos);
            tileMapCell.z = 0;

            if(tileMap.cellBounds.Contains(tileMapCell))
            {
                //Debug.Log("Click inside tilemap!");
                //tileMap.SetTile(tileMapCell, null);
                SCNetworkManager.Client.SendPacket(PacketType.DELETE_TILE, new DeleteTilePacket {
                    Pos = tileMapCell
                });
            }
        }        
    }
}
