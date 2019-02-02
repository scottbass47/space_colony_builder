using Client;
using Shared;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColonistAnimation : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;

    [HideInInspector]
    public Vector2[] Path;

    [HideInInspector]
    public int State;

    private Animator anim;
    private IsometricPosition isoPos;
    private bool FacingRight;

    private Tilemap tilemap;

    void Start()
    {
        anim = GetComponent<Animator>();
        isoPos = GetComponent<IsometricPosition>();

        var eo = GetComponent<EntityObject>();
        eo.AddUpdateListener<PositionUpdate>((position) =>
        {
            Position = position.Pos;
            Position.z = 1;
        });
        eo.AddUpdateListener<PathUpdate>((path) =>
        {
            Path = path.Path;
        });
        eo.AddUpdateListener<StateUpdate>((state) =>
        {
            State = state.State;
        });
    }

    void Update()
    {
        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();
        Vector3 actualPos = isoPos.IsoConversion();

        if (State == (int)EntityState.WALKING)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isMining", false);

            if (actualPos.x > transform.position.x && !FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = true;
            }
            if (actualPos.x < transform.position.x && FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = false;
            }
            if (actualPos.y > transform.position.y) anim.SetBool("isFacingFront", false);
            else if (actualPos.y < transform.position.y) anim.SetBool("isFacingFront", true);  
        }

        else if(State == (int)EntityState.MINING)
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isMining", true);

            Vector3Int cellPos = tilemap.WorldToCell(transform.position);
            Vector3 worldCellPos = tilemap.CellToWorld(cellPos);

            if (worldCellPos.x < transform.position.x && !FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = true;
            }
            if (worldCellPos.x > transform.position.x && FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = false;
            }
        }
    }
}
