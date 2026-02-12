using UnityEngine;

public class Golem : Enemy
{
    [Header("远程攻击")]
    public float attackRangeMin = 2f;
    public float attackRangeMax = 6f;
    public float attackCooldown = 2f;       // ← 新增冷却时间
    public Transform attackPoint;           // 发射点（子物体）
    public GameObject projectilePrefab;     // 拖入 Rock 预制体*/

    public override void Move()
    {
        base.Move();
        //anim.SetBool("jump", false);
    }

    protected override void Awake()
    {
        base.Awake();
        patrolState = new GolemPatrolState();
        chaseState = new GolemChaseState();
        attackState = new GolemAttackState();
    }
}