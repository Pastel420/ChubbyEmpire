using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : BaseState
{
    public float attackTimer;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.anim.SetTrigger("attack");
    }

    public override void LogicUpdate()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            if (currentEnemy.FoundPlayer())
                currentEnemy.SwitchState(NPCState.Chase);
            else
                currentEnemy.SwitchState(NPCState.Patrol);
        }
    }

    public override void PhysicsUpdate() { }
    public override void OnExit() { }
}