using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;

    [Header("¼ì²â²ÎÊý")]
    public bool manual;

    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRaduis;
    public LayerMask groundLayer;

    [Header("×´Ì¬")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;

    private void Start()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if(!manual)
        {
            rightOffset=new Vector2((coll.bounds.size.x+coll.offset.x)/2, coll.bounds.size.y/2);
            leftOffset=new Vector2(-(coll.bounds.size.x+coll.offset.x)/2, coll.bounds.size.y/2);
        }
    }
    private void Update()
    {
        Check();
    }
    public void Check()
    {
        //CheckGround
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
        //CheckWall
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
}
