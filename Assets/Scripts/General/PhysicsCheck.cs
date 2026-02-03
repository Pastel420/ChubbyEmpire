using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    [Header("¼ì²â²ÎÊý")]
    public Vector2 bottomOffset;
    public float checkRaduis;
    public LayerMask groundLayer;

    [Header("×´Ì¬")]
    public bool isGround;

    private void Update()
    {
        Check();
    }
    public void Check()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
    }
}
