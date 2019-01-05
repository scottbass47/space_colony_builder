using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    //Some sort of data/type field
    public Transform WindowPrefab;

    private static GameObject Window;
    private void Start()
    {
        Window = Instantiate(WindowPrefab).gameObject;
        Window.SetActive(false);
    }

    public void DisplayWindow()
    {
        //Set Text (Name and Info)
        //Set Collider to width and height of rect transform
        //Set Background Image
    }
}
