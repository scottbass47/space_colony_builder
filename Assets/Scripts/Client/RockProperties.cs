using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RockProperties : MonoBehaviour
{
    [HideInInspector]
    public int Ore;
    [HideInInspector]
    public int originalOre;
    private bool origSet;

    [HideInInspector]
    public int workers;

    [HideInInspector]
    public bool[] hasSlot;

    [HideInInspector]
    public GameObject healthBar;
    public Transform healthBarPrefab;
    private bool hasHealthBar;

    void Start()
    {
        Ore = 1000;
        hasHealthBar = false;
        origSet = false;
        hasSlot = new bool[4];

        workers = 0;
        var eo = GetComponent<EntityObject>();
        eo.AddUpdateListener<OreUpdate>(
            (ore) =>
            {
                this.Ore = ore.Amount;
                if (!origSet)
                {
                    originalOre = ore.Amount;
                    origSet = true;
                }
                if (origSet && Ore < originalOre && !hasHealthBar) CreateHealthBar();
                });
    }

    private void Update()
    {
        if(hasHealthBar) healthBar.GetComponentInChildren<Slider>().value = (float)Ore / originalOre;
    }

    public void CreateHealthBar()
    {
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        TilemapObject to = GetComponent<TilemapObject>();
        var rockPosition = tilemap.CellToWorld(new Vector3Int(to.Pos.x + 1, to.Pos.y + 1, to.Pos.z));

        healthBar = Instantiate(healthBarPrefab, transform).gameObject;
        healthBar.SetActive(true);
        healthBar.transform.position = new Vector3(rockPosition.x, rockPosition.y + .25f, 1);

        hasHealthBar = true;
    }

}
