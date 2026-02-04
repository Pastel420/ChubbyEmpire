using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new GolemPatrolState();
        chaseState = new GolemChaseState();
    }
}
