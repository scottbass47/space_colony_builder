using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseProperties : MonoBehaviour
{
    [HideInInspector]
    public int[] Residents;

    [HideInInspector]
    public Color houseColor;

    private Selectable selectable;
    private Tilemap tilemap;

    public Sprite[] outlines = new Sprite[4];
    private GameObject outline;

    private void Awake()
    {
        houseColor = Random.ColorHSV();

        selectable = GetComponent<Selectable>();

        EntityObject eo = GetComponent<EntityObject>();

        eo.AddUpdateListener<HouseUpdate>((residents) => {
            this.Residents = residents.Residents;
            Debug.Log($"House residents {string.Join(",", residents.Residents)}");
            SetResidentsColor();
        });
    }

    private void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();

        //Adding outline object from Selectable
        outline = selectable.outline;
        outline.GetComponent<SpriteRenderer>().sprite = outlines[3];
    }

    private void Update()
    {
        if (!selectable.enabled) SetHouseColor();
    }

    public void SetHouseColor()
    {
        var to = GetComponent<TilemapObject>();
        
        if(tilemap!= null) tilemap.SetColor(new Vector3Int(to.Pos.x, to.Pos.y, 1), houseColor);
    }

    private void SetResidentsColor()
    {
        foreach(int r in Residents)
        {
            var go = Game.Instance.EntityManager.GetEntity(r);
            go.GetComponent<SpriteRenderer>().color = houseColor;
        }
    }
}
