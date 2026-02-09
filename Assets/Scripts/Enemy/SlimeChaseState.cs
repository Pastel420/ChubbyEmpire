using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        
        currentEnemy.CurrentSpeed= currentEnemy.ChaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }
    public override void LogicUpdate()
    {
        // 如果非常接近玩家（比如 < 1.0f）→ 切 Attack
        if (currentEnemy.DistanceToTarget() < 1f)
        {
            currentEnemy.SwitchState(NPCState.Attack);
        }
        if (currentEnemy.lostTimeCounter<=0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) ||
            (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x,1,1);
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
