using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatyNumberControl : MonoBehaviour
{
    private float velocity;
    private float opacity;
    public int value;

    private TextMeshProUGUI numText;

    void Start()
    {
        opacity = 1;
        velocity = Random.Range(0f, 0.5f);
        numText = GetComponentInChildren<TextMeshProUGUI>();
        numText.text = "+" + value;
    }

    
    void Update()
    {
        opacity -= .03f;
        numText.color = new Color(1, 1, 1, opacity);
        transform.position += Vector3.up * velocity * Time.deltaTime;
        if (opacity <= 0) Destroy(gameObject);
    }
}
