using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("玩家状态")]
    public bool hasBomb = false;           // 是否有炸弹
    public bool bossDefeated = false;      // Boss是否被击败
    public int currentHealth = 3;          // 当前血量
    public Vector3 respawnPosition;        // 重生点

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 重置关卡相关数据（进入新关卡时调用）
    public void ResetLevelData()
    {
        hasBomb = false;
        // bossDefeated 通常在返回标题时才重置，或者根据你的设计调整
    }

    // 完全重置（返回标题界面）
    public void ResetAllData()
    {
        hasBomb = false;
        bossDefeated = false;
        currentHealth = 3;
    }
}