using UnityEngine;

public class GolemAttackState : BaseState
{
    private float attackTimer;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        // 初次进入立即攻击（可选）
        TryAttack();
        attackTimer = (currentEnemy as Golem)?.attackCooldown ?? 2f;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.attacker != null)
        {
            float playerX = currentEnemy.attacker.position.x;
            float enemyX = currentEnemy.transform.position.x;

            bool shouldFaceRight = playerX > enemyX;          // 玩家在右边 → 应该面朝右
            bool isCurrentlyFacingRight = currentEnemy.transform.localScale.x > 0; // 当前是否面朝右

            if (shouldFaceRight != isCurrentlyFacingRight)
            {
                // 手动翻转：直接设置 localScale.x 为 ±1
                currentEnemy.transform.localScale = new Vector3(
                    shouldFaceRight ? 1 : -1,
                    1,
                    1
                );
            }
        }

        // 检查是否还在攻击范围内
        if (!currentEnemy.FoundPlayer() ||
            currentEnemy.DistanceToTarget() < (currentEnemy as Golem)?.attackRangeMin ||
            currentEnemy.DistanceToTarget() > (currentEnemy as Golem)?.attackRangeMax)
        {
            currentEnemy.SwitchState(NPCState.Chase);
            return;
        }

        // 攻击冷却
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            TryAttack();
            attackTimer = (currentEnemy as Golem)?.attackCooldown ?? 2f;
        }
    }

    private void TryAttack()
    {
        if (currentEnemy is Golem golem)
        {
            // 播放动画
            currentEnemy.anim.SetTrigger("attack");

            // 发射石头
            if (golem.projectilePrefab != null && golem.attackPoint != null)
            {
                GameObject rock = Object.Instantiate(
                    golem.projectilePrefab,
                    golem.attackPoint.position,
                    Quaternion.identity
                );

                // 设置方向：从 attackPoint 指向玩家
                if (currentEnemy.attacker != null)
                {
                    Vector2 dir = (currentEnemy.attacker.position - golem.attackPoint.position);
                    rock.GetComponent<RockProjectile>()?.SetDirection(dir);
                }
            }
        }
    }

    public override void PhysicsUpdate() { }
    public override void OnExit() { }
}