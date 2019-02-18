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
    private int nodeNum;

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
        

        Vector3 dir = Vector3.zero;
         
         if (nodeNum == 0)
         {
             transform.position = actualPos;
             nodeNum++;
         }
         else if(nodeNum == Path.Length - 1)
         {
            if ((actualPos - transform.position).magnitude > .25f)
                dir = actualPos - transform.position;
            else
                transform.position = actualPos;
         }
         else
         {       
             Debug.Log("Node " + nodeNum + ": " + Path[nodeNum]);
             var curNode = transform.position;
             var nextNode = isoPos.PathIsoConversion(Path[nodeNum]);
             Debug.Log("Current: " + curNode + " | Next: " + nextNode + " | Actual: " + actualPos);


             dir = new Vector3(nextNode.x - curNode.x, nextNode.y - curNode.y, 1);
             if (tilemap.WorldToCell(actualPos) == new Vector3Int((int)Path[nodeNum].x, (int)Path[nodeNum].y, 1))
             {
                 nodeNum++;
             }
         }

        transform.Translate(dir * (Constants.COLONIST_SPEED)* Time.deltaTime);



    }
}
