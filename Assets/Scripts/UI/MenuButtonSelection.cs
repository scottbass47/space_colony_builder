using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MenuButtonSelection : MonoBehaviour
{
    private Button thisButton;
    public Button houseButton;

    private PlaceHouse houseScript;
    private ArrayList children;
    private bool buttonsEnabled;


    void Start()
    {
        buttonsEnabled = false;
        houseScript = GetComponent<PlaceHouse>();
        thisButton = GetComponent<Button>();

        children = new ArrayList();
        for(int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            children.Add(child);

        }
        thisButton.onClick.AddListener(Toggle);
        houseButton.onClick.AddListener(BuildHouse);
    }

    void Toggle()
    {
        if (buttonsEnabled)
        {
            foreach (GameObject child in children)
                child.SetActive(false);
            buttonsEnabled = false;
        }
        else
        {
            foreach (GameObject child in children)
                child.SetActive(true);
            buttonsEnabled = true;
        }
    }


    void BuildHouse()
    {
        houseScript.enabled = true;
        Toggle();
    }
}
