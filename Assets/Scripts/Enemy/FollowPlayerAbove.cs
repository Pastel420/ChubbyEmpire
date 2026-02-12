
using UnityEngine;

public class FollowPlayerAbove : MonoBehaviour
{
    [Header("Timing")]
    public float attackToJianciDelay = 2f;  // Attack 结束后，等待多久开始 Jianci
    public float jianciToAttackDelay = 3f;  // Jianci 结束后，等待多久开始下一次 Attack

    [Header("Attack")]
    public Vector2 offset = new Vector2(0f, 2f); // 在玩家上方的偏移

    [Header("Jianci")]
    public float jianciDuration = 1.5f;          // Jianci 动画持续时间
    public string defaultAnimation = "Catwalk";  // Jianci 结束后返回的动画

    private Transform playerTransform;
    private Animator anim;
    private Cat catScript;

    private enum State
    {
        AttackPhase1_WaitBeforeMove,   // 播了 attack，等 0.8s 后移动
        AttackPhase2_WaitAfterMove,    // 移动后，等 0.8s 恢复速度
        JianciActive,                  // 正在播放 jianci（期间所有速度 = 0）
        DelayAfterAttack,              // attack 完成后，等待触发 jianci
        DelayAfterJianci               // jianci 完成后，等待触发 attack
    }

    private State currentState = State.DelayAfterJianci; // 初始：先执行 Attack
    private float timer = 0f;
    private Vector3 targetPosition;

    // 保存原始速度
    private float originalNormalSpeed = 0f;
    private float originalChaseSpeed = 0f;
    private float originalCurrentSpeed = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        catScript = GetComponent<Cat>();
        if (catScript == null)
        {
            Debug.LogError("[FollowPlayerAbove] 未找到 Cat 脚本！");
        }
    }

    void Start()
    {
        TryFindPlayer();
        if (playerTransform == null)
        {
            Debug.LogWarning("[FollowPlayerAbove] 未立即找到 Player，将在 Update 中重试。");
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            TryFindPlayer();
            return;
        }

        timer += Time.deltaTime;

        switch (currentState)
        {
            // —————— ATTACK 流程 ——————
            case State.DelayAfterJianci:
                if (timer >= jianciToAttackDelay)
                {
                    if (anim != null) anim.Play("attack", -1, 0f);
                    targetPosition = playerTransform.position + new Vector3(offset.x, offset.y, 0f);
                    currentState = State.AttackPhase1_WaitBeforeMove;
                    timer = 0f;
                }
                break;

            case State.AttackPhase1_WaitBeforeMove:
                if (timer >= 0.8f)
                {
                    transform.position = targetPosition;

                    // 暂停所有速度
                    if (catScript != null)
                    {
                        originalNormalSpeed = catScript.NormalSpeed;
                        originalChaseSpeed = catScript.ChaseSpeed;
                        originalCurrentSpeed = catScript.CurrentSpeed;

                        catScript.NormalSpeed = 0f;
                        catScript.ChaseSpeed = 0f;
                        catScript.CurrentSpeed = 0f;
                    }

                    currentState = State.AttackPhase2_WaitAfterMove;
                    timer = 0f;
                }
                break;

            case State.AttackPhase2_WaitAfterMove:
                if (timer >= 0.8f)
                {
                    // 恢复所有速度
                    if (catScript != null)
                    {
                        catScript.NormalSpeed = originalNormalSpeed;
                        catScript.ChaseSpeed = originalChaseSpeed;
                        catScript.CurrentSpeed = originalCurrentSpeed;
                    }

                    currentState = State.DelayAfterAttack;
                    timer = 0f;
                }
                break;

            // —————— JIANCI 流程 ——————
            case State.DelayAfterAttack:
                if (timer >= attackToJianciDelay)
                {
                    if (anim != null) anim.Play("jianci", -1, 0f);

                    // 暂停所有速度
                    if (catScript != null)
                    {
                        originalNormalSpeed = catScript.NormalSpeed;
                        originalChaseSpeed = catScript.ChaseSpeed;
                        originalCurrentSpeed = catScript.CurrentSpeed;

                        catScript.NormalSpeed = 0f;
                        catScript.ChaseSpeed = 0f;
                        catScript.CurrentSpeed = 0f;
                    }

                    currentState = State.JianciActive;
                    timer = 0f;
                }
                break;

            case State.JianciActive:
                if (timer >= jianciDuration)
                {
                    // 恢复默认动画
                    if (anim != null) anim.Play(defaultAnimation);

                    // 恢复所有速度
                    if (catScript != null)
                    {
                        catScript.NormalSpeed = originalNormalSpeed;
                        catScript.ChaseSpeed = originalChaseSpeed;
                        catScript.CurrentSpeed = originalCurrentSpeed;
                    }

                    currentState = State.DelayAfterJianci;
                    timer = 0f;
                }
                break;
        }
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
}