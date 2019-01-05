using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Client
{
    public class Selectable : MonoBehaviour
    {
        //Some sort of data/type field

        private Tilemap tilemap;

        public void DisplayWindow(GameObject window)
        {
            //Set Text (Name and Info)
            //Set Collider to width and height of rect transform
            //Set Background Image

            if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

            var tilemapObj = this.gameObject.GetComponent<TilemapObject>();
            
            Vector3 worldMousePos = tilemap.CellToWorld(tilemapObj.Pos);

            RectTransform rt = window.GetComponent<RectTransform>();

            float widthOffset = rt.rect.width * rt.lossyScale.x / 2 + 0.5f;
            float heightOffset = rt.rect.height * rt.lossyScale.y / 2;

            Vector3 WindowPos = new Vector3(worldMousePos.x + widthOffset, worldMousePos.y + heightOffset, 1);

            window.SetActive(true);
            window.transform.SetPositionAndRotation(WindowPos, Quaternion.identity);
        }
    }
}