using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new SlimePatrolState();
        chaseState = new SlimeChaseState();
    }
}
