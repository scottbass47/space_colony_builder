using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindow : MonoBehaviour
{
    public Button exitButton;

    private Vector3 origPosition;
    private Vector2 origPivot;
    private RectTransform rt;
    void Start()
    {
        origPosition = gameObject.transform.position;
        rt = gameObject.GetComponent<RectTransform>();
        origPivot = rt.pivot;

        exitButton.onClick.AddListener(DeactivateWindow);
    }

    void DeactivateWindow()
    {
        gameObject.transform.position = origPosition;
        rt.pivot = origPivot;
        gameObject.SetActive(false);
    }
}
