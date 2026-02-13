using UnityEngine;

// 确保在 Enemy.Update() 之后执行，以覆盖其 localScale
[DefaultExecutionOrder(100)]
public class AlwaysFacePlayer : MonoBehaviour
{
    [Header("设置")]
    public float turnDelay = 0.5f;

    [Header("2D 设置")]
    public bool spriteFacesRightByDefault = true; // true=角色默认朝右

    private Transform playerTransform;
    private bool playerFound = false;
    private Enemy enemy; // 仅用于读取 isHurt，不调用其方法

    private float currentFacing = 1f;   // 逻辑朝向：+1=应面右, -1=应面左
    private float targetFacing = 1f;
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
        // 持续查找玩家
        if (!playerFound || playerTransform == null || playerTransform.gameObject == null)
        {
            TryFindPlayer();
        }

        if (!playerFound)
        {
            // 没有玩家时，保持当前朝向（或可选：不更新）
            ApplyFacing();
            return;
        }

        // 计算玩家相对于敌人的方向
        float directionX = playerTransform.position.x - transform.position.x;
        targetFacing = directionX >= 0 ? 1f : -1f;

        // ★★★ 关键：只要玩家存在，就立即确保朝向正确 ★★★
        // 即使正在转身或受击，我们也优先保证方向对（视觉+逻辑一致）
        currentFacing = targetFacing;
        ApplyFacing();

        // 可选：如果你仍想要“转身延迟”的动画效果，可以保留以下逻辑
        // 但为了索敌稳定，建议直接 currentFacing = targetFacing（如上）
        /*
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
            if (targetFacing != currentFacing)
            {
                isTurning = true;
                turnTimer = 0f;
            }
        }
        */
    }

    void ApplyFacing()
    {
        Vector3 newScale = transform.localScale;
        // 根据美术资源方向，转换逻辑朝向为实际 scale.x
        float visualScaleX = spriteFacesRightByDefault
            ? currentFacing          // 美术朝右：逻辑+1 → scale.x = +1（右）
            : -currentFacing;        // 美术朝左：逻辑+1 → scale.x = -1（右）

        // ★ 强制设置，无视 Enemy 的任何修改 ★
        newScale.x = Mathf.Abs(newScale.x) * visualScaleX;
        transform.localScale = newScale;
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerFound = player != null;
        playerTransform = player ? player.transform : null;
    }
}