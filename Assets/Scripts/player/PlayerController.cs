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

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();


        playerAnimation = GetComponent<PlayerAnimation>();


        inputControl = new PlayerInputControl();
        //跳跃
        inputControl.Gameplay.Jump.started += Jump;
        //攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;
     }

   
    private void OnEnable()
    {
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
        playerAnimation.PlayAttack();
        isAttack = true;
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
