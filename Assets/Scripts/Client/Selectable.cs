﻿using Client;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Selectable : MonoBehaviour
{
    [HideInInspector]
    public Tilemap tilemap;
    private EntityObject eo;

    private static GameObject RockInfo;
    private static AddWorkers addWorkers;
    private static GameObject HouseInfo;
    private static HouseUI houseUI;
    private static GameObject MultipleObjsInfo;

    private static TextMeshProUGUI windowTitle;


    public Transform outlinePrefab;
    [HideInInspector]
    public GameObject outline;

    private void Awake()
    {
        outline = Instantiate(outlinePrefab, transform).gameObject;
        outline.SetActive(false);
    }

    private void Update()
    {
        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();
    }

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
        if (RockInfo == null) RockInfo = window.transform.GetChild(3).gameObject;
        if (addWorkers == null) addWorkers = RockInfo.GetComponent<AddWorkers>();

        if (HouseInfo == null) HouseInfo = window.transform.GetChild(4).gameObject;
        if (houseUI == null) houseUI = HouseInfo.GetComponent<HouseUI>();

        if (MultipleObjsInfo == null) MultipleObjsInfo = window.transform.GetChild(5).gameObject;

        if (windowTitle == null) windowTitle = window.GetComponentInChildren<TextMeshProUGUI>();

        eo = gameObject.GetComponent<EntityObject>();

        EntityType type = eo.Type;
        switch (type)
        {
            //@Gross
            case EntityType.ORE:
                windowTitle.text = "ROCK ";// + eo.ID;
                MultipleObjsInfo.SetActive(false);
                HouseInfo.SetActive(false);

                RockInfo.SetActive(true);
                addWorkers.ore = gameObject;
                addWorkers.Refresh();
                break;
            case EntityType.HOUSE:
                windowTitle.text = "HOUSE ";// + eo.ID;
                RockInfo.SetActive(false);
                MultipleObjsInfo.SetActive(false);

                HouseInfo.SetActive(true);
                houseUI.house = gameObject;
                break;
        }
        
        window.SetActive(true);
    }

    public static void DisplayWindow(GameObject window, ArrayList selectedItems)
    {
        if (RockInfo == null) RockInfo = window.transform.GetChild(3).gameObject;
        if (addWorkers == null) addWorkers = RockInfo.GetComponent<AddWorkers>();

        if (HouseInfo == null) HouseInfo = window.transform.GetChild(4).gameObject;
        if (houseUI == null) houseUI = HouseInfo.GetComponent<HouseUI>();

        if (MultipleObjsInfo == null) MultipleObjsInfo = window.transform.GetChild(5).gameObject;

        if (windowTitle == null) windowTitle = window.GetComponentInChildren<TextMeshProUGUI>();


        windowTitle.text = selectedItems.Count + " ROCKS";// + eo.ID;
        RockInfo.SetActive(false);
        HouseInfo.SetActive(false);

        MultipleObjsInfo.SetActive(true);

        MultipleObjsInfo.GetComponent<MultipleObjsUI>().Entities = selectedItems;

        window.SetActive(true);
    }
}
