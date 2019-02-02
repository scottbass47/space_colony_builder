using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float yPos;
    public float scale;
    public int val;

    private float origY;
    private float elapsedTime;
    private float originalScale;
    private TextMeshProUGUI numText;

    void Start()
    {
        elapsedTime = 0;
        origY = transform.position.y;
        originalScale = transform.localScale.x;
        numText = GetComponentInChildren<TextMeshProUGUI>();
        numText.text = "+" + val;
    }

    
    void Update()
    {
        if (!GetComponent<Animation>().isPlaying)
        {
            Destroy(gameObject);
        }
        elapsedTime += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, origY + yPos, 0);
        transform.localScale = new Vector3(scale*originalScale, scale*originalScale, 0.0025f);
    }
}
