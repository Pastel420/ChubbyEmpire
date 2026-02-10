using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    public Vector2 inputDirection;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;


    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public int maxJumpCount = 2; // 最大跳跃次数（包括一段跳）
    private int currentJumpCount = 0; // 当前已跳跃次数
    public float hurtForce;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;

    private float doubleJumpForceMultiplier = 0.8f;

    [Header("攻击判定")]
    public GameObject attackHitbox;      // 拖入 AttackHitbox 子物体
    public float attackActiveTime = 0.2f; // 攻击判定持续时间（秒）
    private Coroutine currentAttack;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();

        inputControl = new PlayerInputControl();

        // 注册回调
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.performed += PlayerAttack;

        //inputControl.Enable(); // ←←← 这行最重要！

        
    }


    private void OnEnable()
    {
        //Debug.Log("【PlayerController】输入系统已启用。");
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!isHurt)
            Move();

        // 检查是否落地
        if (physicsCheck.isGround && rb.velocity.y <= 0)
        {
            currentJumpCount = 0;
       
        }
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.fixedDeltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x < 0)
            faceDir = -1;
        if (inputDirection.x > 0)
            faceDir = 1;
        //人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);
    }
    private void Jump(InputAction.CallbackContext context)
    {
        // 如果在地面上，重置跳跃计数
        if (physicsCheck.isGround)
        {
            currentJumpCount = 0;
        }

        // 如果还可以跳跃
        if (currentJumpCount < maxJumpCount)
        {
            // 重置Y轴速度，确保跳跃高度一致
            rb.velocity = new Vector2(rb.velocity.x, 0);

            // 根据跳跃次数计算跳跃力
            float actualJumpForce = jumpForce;

            // 如果是二段跳（currentJumpCount == 1表示已经跳过一次）
            if (currentJumpCount == 1)
            {
                // 应用二段跳力度系数
                actualJumpForce *= doubleJumpForceMultiplier;
            }

            // 执行跳跃
            rb.AddForce(transform.up * actualJumpForce, ForceMode2D.Impulse);

            currentJumpCount++;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (obj.performed) // 这行保留（其实 performed 回调时 obj.alreadyUsed=false）
        {
            Debug.Log("【Attack】G 键按下！");

            playerAnimation.PlayAttack();
            isAttack = true;

            if (attackHitbox != null)
            {
                attackHitbox.SetActive(true);
                StartCoroutine(DisableHitboxAfterDelay());
            }
        }
    }

    private IEnumerator ActivateAttackHitbox()
    {
        // 激活判定区域
        attackHitbox.SetActive(true);

        // 等待一段时间（攻击窗口）
        yield return new WaitForSeconds(attackActiveTime);

        // 关闭判定区域
        attackHitbox.SetActive(false);
    }

    private IEnumerator DisableHitboxAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // 0.2秒后关闭
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x),0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
}
