using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseProperties : MonoBehaviour
{
    //[HideInInspector]
    public int[] Residents;

    [HideInInspector]
    public Color houseColor;

    private Selectable selectable;
    private Tilemap tilemap;

    private void Awake()
    {
        houseColor = Random.ColorHSV();

        selectable = GetComponent<Selectable>();

        EntityObject eo = GetComponent<EntityObject>();

        eo.AddUpdateListener<HouseUpdate>((residents) => {
            this.Residents = residents.Residents;
            SetResidentsColor();
        });
    }

    private void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        //Disable this if you don't want houses to show their house color
        SetHouseColor();
    }

    private void Update()
    {
        if (!selectable.enabled) SetHouseColor();
    }

    public void SetHouseColor()
    {
        var to = GetComponent<TilemapObject>();
        
        tilemap.SetColor(new Vector3Int(to.Pos.x, to.Pos.y, 1), houseColor);
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
