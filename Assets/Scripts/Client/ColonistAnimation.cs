using Client;
using Shared;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColonistAnimation : MonoBehaviour
{
    // Not being used currently. 
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public Vector2[] Path;
    private Vector2 dest;
    private int nodeNum;

    [HideInInspector]
    public int State;

    private Animator anim;
    private IsometricPosition isoPos;
    private bool FacingRight;

    private Tilemap tilemap;

    private bool done = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        isoPos = GetComponent<IsometricPosition>();

        var eo = GetComponent<EntityObject>();

        // Set the initial position
        var pos = eo.GetEData<EPositionData>().Pos;
        this.position = pos;
        this.position.z = 1;
        isoPos.SetPosition(this.position);

        eo.AddUpdateListener<PositionUpdate>((position) =>
        {
            this.position = position.Pos;
            this.position.z = 1;
            velocity = position.Vel;
        });
        eo.AddUpdateListener<PathUpdate>((path) =>
        {
            done = false;
            Path = path.Path;
            dest = path.Dest;
            nodeNum = 0;
        });
        eo.AddUpdateListener<StateUpdate>((state) =>
        {
            State = state.State;
        });
    }

    void Update()
    {
        if (tilemap == null) tilemap = FindObjectOfType<Tilemap>();

        Vector3 deltaPos = Vector3.zero;
        if (!done)
        {
            Vector3 p0 = isoPos.Position;
            int currentNode = getNodeIndex(p0);
            if (currentNode == -1)
            {
                //Debug.Log("Not on path!");
                return;
            }

            var dir = Vector3.zero;
            var finalStretch = currentNode == Path.Length - 1;

            if (finalStretch)
            {
                dir = vec2To3(dest) - p0;
            } else
            {
                dir = vec2To3(Path[currentNode + 1]) - p0;
            }

            dir.Normalize();

            // Here we can adjust the speed if we're running behind the server
            var p1 = p0 + dir * Constants.COLONIST_SPEED * Time.deltaTime;
            isoPos.SetPosition(p1);

            if (finalStretch)
            {
                var error = (isoPos.Position - vec2To3(dest)).sqrMagnitude;

                // We've reached the end of the path, so we should stop trying to move
                if (error < 0.005f)
                {
                    //Debug.Log("Pathing complete.");
                    done = true;

                    // This won't scale for other animations. Maybe the server sends Job info when
                    // colonist is assigned a new job so the client can predict what animation it
                    // should play when it's done pathfinding.
                    SwitchToMining();
                }
            }
            
            deltaPos = isoPos.IsoToWorld(p1) - isoPos.IsoToWorld(p0);
        }

        if (!done)
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isMining", false);

            if (deltaPos.x > 0 && !FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = true;
            }
            if (deltaPos.x < 0 && FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = false;
            }
            if (deltaPos.y > 0) anim.SetBool("isFacingFront", false);
            else if (deltaPos.y < 0) anim.SetBool("isFacingFront", true);  
        }

        
    }

    private void SwitchToMining()
    {
        anim.SetBool("isRunning", false);
        anim.SetBool("isMining", true);

        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        Vector3 worldCellPos = tilemap.CellToWorld(cellPos);

        float animationOffset = -0.05f;
        if (worldCellPos.x < transform.position.x)
        {
            isoPos.Translate(isoPos.WorldToIso(new Vector3(animationOffset, 0f)));
            if(!FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = true;
            }
        }
        if (worldCellPos.x > transform.position.x)
        {
            isoPos.Translate(isoPos.WorldToIso(new Vector3(-animationOffset, 0f)));
            if (FacingRight)
            {
                transform.Rotate(Vector3.up, 180);
                FacingRight = false; 
            }
        }
    }

    private Vector3 vec2To3(Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 1);
    }

    private int getNodeIndex(Vector3 pos)
    {
        var gridPos = new Vector2((int)(pos.x + .5f), (int)(pos.y + .5f));

        for(int i =0; i < Path.Length; i++)
            if (Path[i] == gridPos)
                return i;
       
        //Shouldn't happen please
        return -1;
    }
}
