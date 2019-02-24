using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskObject : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;

    public void SetTitle(string title)
    {
        titleText.text = title.ToUpper();
    }

    public void SetStatus(string status)
    {
        statusText.text = status.ToUpper();
    }
}
