using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePatrolState : BaseState
{   
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.CurrentSpeed = currentEnemy.NormalSpeed;
    }
    public override void LogicUpdate()
    {
        if(currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) ||
            (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(-currentEnemy.transform.localScale.x,1,-1);
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }
    public override void PhysicsUpdate()
    {
       // throw new System.NotImplementedException();
    }
    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
    }
}
