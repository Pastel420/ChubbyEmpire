// RockProjectile.cs
using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [Header("属性")]
    public float speed;
    public int damage;
    public float destroyDelay; // 安全兜底：5秒后自动销毁

    private Vector2 direction;

    void Start()
    {
        // 安全兜底：防止无限飞行
        Destroy(gameObject, destroyDelay);
    }

    // 由 GolemAttackState 调用，设置发射方向
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        // 直线匀速飞行
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 打中玩家
        /*if (other.CompareTag("Player"))
        {
            // 调用玩家受伤方法（你需要确保 Player 有这个方法）
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }*/

        // 打中墙壁（假设墙壁在 "Ground" 层，或使用特定 Tag）
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.CompareTag("Wall"))
        {
            // 可选：播放粒子/音效
        }

        // 无论打中谁，都销毁自己
        Destroy(gameObject);
    }
}