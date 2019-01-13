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

    private void Start()
    {
        houseColor = Random.ColorHSV();
        //Disable this if you don't want houses to show their house color
        SetHouseColor();

        EntityObject eo = GetComponent<EntityObject>();

        eo.AddUpdateListener<HouseUpdate>((residents) => {
            this.Residents = residents.Residents;
            SetResidentsColor();
            });
    }

    public void SetHouseColor()
    {
        var to = GetComponent<TilemapObject>();
        var tilemap = FindObjectOfType<Tilemap>();
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
