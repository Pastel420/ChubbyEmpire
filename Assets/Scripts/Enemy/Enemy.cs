using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    public Animator anim;
    public PhysicsCheck physicsCheck;
    [Header("基本参数")]

    public float NormalSpeed;
    public float ChaseSpeed;
    public float CurrentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checksize;
    public LayerMask attackLayer;
    public float CheckDistance;

    

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTimeCounter;
    public float lostTime;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState attackState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        CurrentSpeed = NormalSpeed;
        physicsCheck= GetComponent<PhysicsCheck>();
        waitTimeCounter = waitTime;
        Character character = GetComponent<Character>();
        if (character != null)
        {
            character.OnDie.AddListener(OnDie);          // 死亡时调用 Enemy.OnDie()
            character.OnTakeDamage.AddListener(OnTakeDamage); // 受伤时调用 Enemy.OnTakeDamage()
        }
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    public void Update()
    {
        ValidateAttacker(); // ← 新增这一行
        faceDir =new Vector3(transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }

    void ValidateAttacker()
    {
        // 如果 attacker 是 Player，并且 Player 还活着，就保留
        if (attacker != null &&
            attacker.CompareTag("Player") &&
            attacker.gameObject.activeInHierarchy)
        {
            return; // 合法，不用管
        }

        // 否则：清空 attacker，让 AI 重新索敌
        attacker = null;
    }

    public void FixedUpdate()
    {
        if (!isHurt && !isDead && !wait)
        {
            Move();
        }
        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(CurrentSpeed * faceDir.x*Time.deltaTime, rb.velocity.y);
    }

    public bool FoundPlayer()
    {
        
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checksize, 0, faceDir, CheckDistance, attackLayer);
        
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }

        if(!FoundPlayer()&&lostTimeCounter>0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
    }

    public void OnTakeDamage(Transform attackTrans)
    {
        attacker= attackTrans;
        //Turn
        if(attackTrans.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if(attackTrans.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //Hurt
        isHurt= true;
        anim.SetTrigger("hurt");
        Vector2 dir=new Vector2(transform.position.x - attackTrans.position.x,0).normalized;
        rb.velocity= new Vector2(0,rb.velocity.y);
        StartCoroutine(Onhurt(dir));
    }

    private IEnumerator Onhurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;

        // ★ 新增：受击结束后，强制重置 attacker 并尝试恢复追击 ★
        attacker = null; // 清掉旧引用，避免无效目标
        if (FoundPlayer())
        {
            SwitchState(NPCState.Chase);
        }
    }

    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead= true;

        //记得回来改
        //*******************************************************************************************
        Destroy(gameObject, 1f);
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(CheckDistance* -transform.localScale.x,0),0.2f);
    }

    public float DistanceToTarget()
    {
        if (attacker == null)
        {
            return float.MaxValue;
        }
        return Vector2.Distance(transform.position, attacker.position);
    }
}
