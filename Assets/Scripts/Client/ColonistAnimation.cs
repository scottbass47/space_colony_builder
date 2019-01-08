using Client;
using Shared;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 actualPos = isoPos.IsoConversion();

        if (State == (int)EntityState.WALKING)
        {
            anim.SetBool("isWalking", true);
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
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsMining", true);
        }
    }
}
