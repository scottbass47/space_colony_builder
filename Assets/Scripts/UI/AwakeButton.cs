using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwakeButton : MonoBehaviour
{
    public Button thisButton;

    private MenuButtonSelection script;

    // Start is called before the first frame update
    void Start()
    {
        this.thisButton.onClick.AddListener(TurnOnButton);
        script = GetComponent<MenuButtonSelection>();
    }

    void TurnOnButton()
    {
        script.enabled = true;
        this.enabled = false;
    }
}
