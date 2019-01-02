using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonSelection : MonoBehaviour
{
    public Button thisButton;
    public Button houseButton;
    public Button hospitalButton;

    private ArrayList children;
    private bool enabled;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        children = new ArrayList();
        for(int i = 1; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
        thisButton.onClick.AddListener(Toggle);
        houseButton.onClick.AddListener(Build);
        hospitalButton.onClick.AddListener(Toggle);
    }

    // Update is called once per frame
    void Update()
    {
       
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

    void Build()
    {
        Toggle();
    }
}
