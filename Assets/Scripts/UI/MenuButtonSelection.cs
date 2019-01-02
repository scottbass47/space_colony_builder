﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MenuButtonSelection : MonoBehaviour
{
    public Button thisButton;
    public Button houseButton;
    public Button hospitalButton;

    private PlaceHouse houseScript;
    private ArrayList children;
    private bool enabled;


    void Start()
    {
        enabled = false;
        houseScript = GetComponent<PlaceHouse>();

        children = new ArrayList();
        for(int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            children.Add(child);

        }
        thisButton.onClick.AddListener(Toggle);
        houseButton.onClick.AddListener(BuildHouse);
        hospitalButton.onClick.AddListener(BuildHospital);
    }

    void Toggle()
    {
        if (enabled)
        {
            foreach (GameObject child in children)
                child.SetActive(false);
            enabled = false;
        }
        else
        {
            foreach (GameObject child in children)
                child.SetActive(true);
            enabled = true;
        }
    }


    void BuildHouse()
    {
        houseScript.enabled = true;
    }
    void BuildHospital()
    {
        
    }
}
