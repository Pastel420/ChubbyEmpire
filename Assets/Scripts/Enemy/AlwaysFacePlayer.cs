using UnityEngine;

// 确保在 Enemy.Update() 之后执行，以覆盖其 localScale
[DefaultExecutionOrder(100)]
public class AlwaysFacePlayer : MonoBehaviour
{
    [Header("设置")]
    public float turnDelay = 1f; // 转身所需等待时间（秒）

    [Header("2D 设置")]
    public bool spriteFacesRightByDefault = true; // true = 美术资源默认朝右

    private Transform playerTransform;
    private bool playerFound = false;
    private Enemy enemy; // 仅用于读取 isHurt（当前未使用，但保留扩展性）

    private float currentFacing = 1f;   // 当前实际朝向（+1 右，-1 左）
    private float targetFacing = 1f;    // 目标朝向（基于玩家位置）
    private bool isTurning = false;
    private float turnTimer = 0f;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            enabled = false;
            return;
        }

        currentFacing = 1f;
        TryFindPlayer();
    }

    void Update()
    {
        // 持续尝试查找玩家
        if (!playerFound || playerTransform == null || playerTransform.gameObject == null)
        {
            TryFindPlayer();
        }

        if (!playerFound)
        {
            ApplyFacing();
            return;
        }

        // 计算玩家在敌人左侧还是右侧
        float directionX = playerTransform.position.x - transform.position.x;
        targetFacing = directionX >= 0 ? 1f : -1f;

        // 转身延迟逻辑
        if (isTurning)
        {
            turnTimer += Time.deltaTime;
            if (turnTimer >= turnDelay)
            {
                currentFacing = targetFacing;
                ApplyFacing();
                isTurning = false;
            }
        }
        else
        {
            // 如果目标朝向与当前不同，开始转身计时
            if (targetFacing != currentFacing)
            {
                isTurning = true;
                turnTimer = 0f;
            }
        }

        // 注意：在转身完成前，currentFacing 不变，因此 ApplyFacing 会保持旧朝向
        ApplyFacing();
    }

    void ApplyFacing()
    {
        Vector3 newScale = transform.localScale;

        // 根据美术资源默认朝向，转换逻辑朝向为实际 scale.x
        float visualScaleX = spriteFacesRightByDefault
            ? currentFacing          // 美术朝右：逻辑 +1 → scale.x = +1（显示朝右）
            : -currentFacing;        // 美术朝左：逻辑 +1 → scale.x = -1（显示朝右）

        // 强制设置 X 缩放，保留 Y/Z 不变
        newScale.x = Mathf.Abs(transform.localScale.x) * visualScaleX;
        transform.localScale = newScale;
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerFound = player != null;
        playerTransform = player ? player.transform : null;
    }
}