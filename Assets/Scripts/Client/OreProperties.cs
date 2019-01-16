using Client;
using Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class OreProperties : MonoBehaviour
{
    private Tilemap tilemap;
    private TilemapObject to;

    //Ore Management
    [HideInInspector]
    public int Ore;
    [HideInInspector]
    public int originalOre;
    private bool origSet;
    public Transform floatyNumber;

    //Worker Slot Management
    [HideInInspector]
    public bool[] hasSlot;

    //Health Bar and Ore Deterioration
    [HideInInspector]
    public GameObject healthBar;
    public Transform healthBarPrefab;
    private bool hasHealthBar;
    public TileBase[] oreTiles = new TileBase[4];
    
    //Outline
    public Sprite[] outlines = new Sprite[4];
    private GameObject outline;

    void Start()
    {
        hasHealthBar = false;
        origSet = false;
        hasSlot = new bool[4];

        //Adding event listener to handle ore amount and health bar creation
        var eo = GetComponent<EntityObject>();
        eo.AddUpdateListener<OreUpdate>(
            (ore) =>
            {
                if(origSet) CreateFloatyNumber(this.Ore - ore.Amount);

                this.Ore = ore.Amount;
                if (!origSet)
                {
                    originalOre = ore.Amount;
                    origSet = true;
                }
                if (origSet && Ore < originalOre && !hasHealthBar) CreateHealthBar();
            });

        //Adding outline object from Selectable
        outline = GetComponent<Selectable>().outline;

        to = GetComponent<TilemapObject>();
    }

    private void Update()
    {
        //Health related updates
        if (hasHealthBar)
        {
            var orePercent = (float)Ore / originalOre;
            healthBar.GetComponentInChildren<Slider>().value = orePercent;
            Vector3Int tilePos = to.Pos;
            tilePos.z = 1;

            if(orePercent < .25f)
            {
                outline.GetComponent<SpriteRenderer>().sprite = outlines[3];
                tilemap.SetTile(tilePos, oreTiles[3]);
            }
            else if (orePercent < .50f)
            {
                outline.GetComponent<SpriteRenderer>().sprite = outlines[2];
                tilemap.SetTile(tilePos, oreTiles[2]);
            }
            else if (orePercent < .75f)
            {
                tilemap.SetTile(tilePos, oreTiles[1]);
                outline.GetComponent<SpriteRenderer>().sprite = outlines[1];
            }

        }
        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();
    }

    public void CreateHealthBar()
    {
        var rockPosition = tilemap.CellToWorld(new Vector3Int(to.Pos.x + 1, to.Pos.y + 1, to.Pos.z));

        healthBar = Instantiate(healthBarPrefab, transform).gameObject;
        healthBar.SetActive(true);
        healthBar.transform.position = new Vector3(rockPosition.x, rockPosition.y + .25f, 1);

        hasHealthBar = true;
    }

    public void CreateFloatyNumber(int val)
    {
        var num = Instantiate(floatyNumber, transform).gameObject;
        var controller = num.GetComponent<FloatyNumberControl>();
        controller.value = val;
        Vector3 numPos = outline.transform.position;
        numPos.x += Random.Range(-.10f, .25f);
        numPos.y += Random.Range(-.10f, .10f);
        num.transform.SetPositionAndRotation(numPos, Quaternion.identity);
    }

}
