using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    public Vector2 inputDirection;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private PlayerAudio playerAudio;

    [Header("基本参数")]
    public float speed;

    // 使用 NonSerialized 防止 Inspector 干扰
    [NonSerialized]
    public float jumpForce;

    public int maxJumpCount = 2;
    private int currentJumpCount = 0;
    public float hurtForce;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;

    private float doubleJumpForceMultiplier = 0.8f;

    [Header("攻击判定")]
    public GameObject attackHitbox;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAudio = GetComponent<PlayerAudio>();

        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.performed += PlayerAttack;

        // 初始检测
        UpdateJumpForceByLoadedScenes();

        // 监听场景加载/卸载
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        // inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!isHurt)
            Move();

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
        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            currentJumpCount = 0;
        }

        if (currentJumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            float actualJumpForce = jumpForce;
            if (currentJumpCount == 1)
            {
                actualJumpForce *= doubleJumpForceMultiplier;
            }
            rb.AddForce(transform.up * actualJumpForce, ForceMode2D.Impulse);
            currentJumpCount++;

            if (playerAudio != null)
                playerAudio.PlayJumpSound();
        }
    }

    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        if (isDead || isAttack) return;
        playerAnimation.PlayAttack();
        isAttack = true;
        if (playerAudio != null)
            playerAudio.PlayAttackSound();
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        if (playerAudio != null)
            playerAudio.PlayHurtSound();
    }

    public void PlayerDead()
    {
        isDead = true;
        if (isAttack)
        {
            isAttack = false;
            DisableAttackHitbox();
        }
    }

    public void PlayItemPickupSound()
    {
        if (playerAudio != null)
            playerAudio.PlayItemSound();
    }

    public void PlayDoorOpenSound()
    {
        if (playerAudio != null)
            playerAudio.PlayDoorSound();
    }

    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            Debug.Log("攻击判定开启");
        }
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
            isAttack = false;
        }
    }

    public void Revive()
    {
        isDead = false;
        isHurt = false;
        isAttack = false;
        inputControl.Gameplay.Enable();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        Debug.Log("玩家已复活");
    }

    // 核心逻辑：检查所有已加载场景，只要有一个包含 "C"，就设为 60
    private void UpdateJumpForceByLoadedScenes()
    {
        bool hasSceneWithC = false;

        // 遍历所有已加载的场景
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.IsValid() && scene.name.Contains("C"))
            {
                hasSceneWithC = true;
                break; // 找到一个就够了
            }
        }

        jumpForce = hasSceneWithC ? 50f : 45f;

        // 调试日志
        string allSceneNames = "";
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            allSceneNames += SceneManager.GetSceneAt(i).name + ", ";
        }
        Debug.Log($"已加载场景: [{allSceneNames}] | 包含 C 的场景? {hasSceneWithC} | jumpForce = {jumpForce}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateJumpForceByLoadedScenes();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        UpdateJumpForceByLoadedScenes();
    }
}