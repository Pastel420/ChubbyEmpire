using UnityEngine;

public class AlwaysFacePlayer : MonoBehaviour
{
    [Header("设置")]
    public bool use2DFlip = true;
    public float turnDelay = 0.5f;

    [Header("2D 设置")]
    public bool spriteFacesRightByDefault = true; // ★关键：你的角色默认朝哪边？

    private Transform playerTransform;
    private bool playerFound = false;

    private float currentFacing = 1f;
    private float targetFacing = 1f;
    private bool isTurning = false;
    private float turnTimer = 0f;

    void Start()
    {
        // 根据默认朝向初始化 currentFacing
        currentFacing = spriteFacesRightByDefault ? 1f : -1f;
        TryFindPlayer();
    }

    void Update()
    {
        if (!playerFound || playerTransform == null || playerTransform.gameObject == null)
        {
            TryFindPlayer();
        }

        if (!playerFound) return;

        if (use2DFlip)
        {
            float directionX = playerTransform.position.x - transform.position.x;
            targetFacing = directionX >= 0 ? 1f : -1f;

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
        }
        else
        {
            transform.LookAt(playerTransform);
        }
    }

    void ApplyFacing()
    {
        Vector3 newScale = transform.localScale;
        // ★核心修复：根据 spriteFacesRightByDefault 决定实际 scale.x★
        float visualFacing = spriteFacesRightByDefault
            ? currentFacing          // 默认朝右：+1=右, -1=左
            : -currentFacing;        // 默认朝左：+1=左, -1=右（反转）

        newScale.x = Mathf.Abs(newScale.x) * visualFacing;
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