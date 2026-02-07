using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    public override void Move()
    {
        base.Move();
        anim.SetBool("jump", false);
    }
    protected override void Awake()
    {
        base.Awake();
        patrolState = new SlimePatrolState();
        chaseState = new SlimeChaseState();
    }
}
