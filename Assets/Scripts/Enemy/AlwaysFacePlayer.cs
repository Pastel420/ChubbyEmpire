using UnityEngine;

public class AlwaysFacePlayer : MonoBehaviour
{
    [Header("设置")]
    public bool use2DFlip = true;           // true = 2D 翻转（仅 X 轴），false = 3D LookAt
    public float turnDelay = 0.5f;          // 检测到需转身后的延迟时间（秒）

    private Transform playerTransform;
    private bool playerFound = false;

    // 2D 转身延迟相关
    private float currentFacing = 1f;       // 当前显示的朝向：1=右, -1=左
    private float targetFacing = 1f;        // 玩家所在侧的目标朝向
    private bool isTurning = false;         // 是否正在等待转身
    private float turnTimer = 0f;           // 转身计时器

    void Start()
    {
        // 初始化当前朝向（从当前 scale 读取）
        currentFacing = Mathf.Sign(transform.localScale.x);
        if (currentFacing == 0) currentFacing = 1f;
        TryFindPlayer();
    }

    void Update()
    {
        // 每帧都尝试找玩家（即使之前找到了，也防玩家重生后引用失效）
        if (!playerFound || playerTransform == null || playerTransform.gameObject == null)
        {
            TryFindPlayer();
        }

        if (!playerFound)
        {
            return; // 找不到玩家，跳过朝向逻辑
        }

        if (use2DFlip)
        {
            // 计算玩家在左边还是右边
            float directionX = playerTransform.position.x - transform.position.x;
            targetFacing = directionX >= 0 ? 1f : -1f;

            if (isTurning)
            {
                // 正在等待转身
                turnTimer += Time.deltaTime;
                if (turnTimer >= turnDelay)
                {
                    // 延迟结束，执行翻转
                    currentFacing = targetFacing;
                    ApplyFacing();
                    isTurning = false;
                }
            }
            else
            {
                // 不在转身中
                if (targetFacing != currentFacing)
                {
                    // 需要转身 → 启动延迟
                    isTurning = true;
                    turnTimer = 0f;
                    // 注意：此时不翻转，继续用 currentFacing 显示
                }
            }
        }
        else
        {
            // 3D 模式：直接 LookAt（无延迟）
            transform.LookAt(playerTransform);
        }
    }

    void ApplyFacing()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * currentFacing;
        transform.localScale = newScale;
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerFound = true;
        }
        else
        {
            playerFound = false;
        }
    }
}