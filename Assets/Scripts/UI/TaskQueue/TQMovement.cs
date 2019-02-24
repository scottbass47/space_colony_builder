using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TQMovement : MonoBehaviour
{
    public Button HUDButton;
    public GameObject scrollWindow;

    private RectTransform scrollRect;
    private bool poppedUp;

    void Start()
    {
        HUDButton.onClick.AddListener(displayQueue);
        scrollRect = scrollWindow.GetComponent<RectTransform>();
        poppedUp = false;
    }


    void Update()
    {

    }

    void displayQueue()
    {
        if (!poppedUp)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + scrollRect.rect.height,
                transform.position.z);
            scrollWindow.SetActive(true);
            poppedUp = true;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - scrollRect.rect.height,
                transform.position.z);
            scrollWindow.SetActive(false);
            poppedUp = false;
        }
    }
}
