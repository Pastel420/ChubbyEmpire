using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Boss设置")]
    public float bossMaxHealth = 100f;
    private float currentHealth;

    [Header("传送门")]
    public GameObject portalToEnd;          // 击败Boss后激活的传送门

    void Start()
    {
        currentHealth = bossMaxHealth;
        portalToEnd.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DefeatBoss();
        }
    }

    void DefeatBoss()
    {
        Debug.Log("Boss被击败！");
        PlayerDataManager.Instance.bossDefeated = true;

        // 激活传送门
        portalToEnd.SetActive(true);

        // 播放击败动画/特效
    }
}