using Client;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{
    //Some sort of data/type field

    private Tilemap tilemap;
    private EntityObject eo;

    private GameObject RockInfo;

    public void DisplayPopUpWindow(GameObject window)
    {
        //Set Text (Name and Info)
        //Set Collider to width and height of rect transform
        //Set Background Image

        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

        var tilemapObj = gameObject.GetComponent<TilemapObject>();

        Vector3 worldMousePos = tilemap.CellToWorld(tilemapObj.Pos);

        RectTransform rt = window.GetComponent<RectTransform>();

        float widthOffset = rt.rect.width * rt.lossyScale.x / 2 + 0.3f;
        float heightOffset = rt.rect.height * rt.lossyScale.y / 2;

        Vector3 WindowPos = new Vector3(worldMousePos.x + widthOffset, worldMousePos.y + heightOffset, 1);

        window.SetActive(true);
        window.transform.SetPositionAndRotation(WindowPos, Quaternion.identity);
    }

    public void DisplayWindow(GameObject window)
    {
        if(RockInfo == null) RockInfo = window.transform.GetChild(3).gameObject;

        eo = gameObject.GetComponent<EntityObject>();
        EntityType type = eo.Type;

        var rt = window.GetComponent<RectTransform>();

        switch (type)
        {
            //@Gross
            case EntityType.ROCK:
                window.GetComponentInChildren<TextMeshProUGUI>().text = "ROCK ";// + eo.ID;
                RockInfo.SetActive(true);
                var addWorkers = RockInfo.GetComponent<AddWorkers>();
                addWorkers.rock = gameObject;
                addWorkers.Refresh();
                break;
            case EntityType.HOUSE:
                window.GetComponentInChildren<TextMeshProUGUI>().text = "HOUSE ";// + eo.ID;
                RockInfo.SetActive(false);
                break;
        }
        
        window.SetActive(true);
    }
}
