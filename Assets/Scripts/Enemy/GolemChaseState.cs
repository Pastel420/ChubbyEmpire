using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("Enter Chase State");
        currentEnemy.CurrentSpeed= currentEnemy.ChaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }
    public override void LogicUpdate()
    {
        // 先检查是否还看到玩家
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
            return;
        }
        // 尝试将 currentEnemy 转为 Golem
        if (currentEnemy is Golem golem)
        {
            float dist = currentEnemy.DistanceToTarget();
            if (dist >= golem.attackRangeMin && dist <= golem.attackRangeMax)
            {
                currentEnemy.SwitchState(NPCState.Attack);
                return; 
            }
        }
        if (!currentEnemy.physicsCheck.isGround ||
            (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) ||
            (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        //throw new System.NotImplementedException();
    }
    public override void OnExit()
    {
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("run", false);
    }
}
