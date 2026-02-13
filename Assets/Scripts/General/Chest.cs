using UnityEngine;

public class Chest : MonoBehaviour, Iinteractable
{
    private SpriteRenderer spriteRenderer;

    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    [Header("物品设置")]
    public ItemType itemType = ItemType.Bomb;
    [TextArea]
    public string itemDescription = "获得了物品！";

    public enum ItemType
    {
        Bomb,       // 炸弹
        HealthPotion, // 回血药水
        Key
    }

    [Header("药水设置")]
    public int healAmount = 1;              // 恢复血量

    [Header("音效")]
    public AudioClip openSound;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            OpenChest();
        }
        else
        {
            Debug.Log("宝箱已经空了");
        }
    }

    private void OpenChest()
    {
        // 视觉反馈
        spriteRenderer.sprite = openSprite;
        isDone = true;

        // 播放音效
        if (openSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(openSound);
        }
        else
        {
            // 使用默认交互音效
            FindObjectOfType<UISoundPlayer>()?.PlayInteractSuccess();
        }
        // 给予物品
        GiveItemToPlayer();

        // 移除交互标签
        this.gameObject.tag = "Untagged";

        //Debug.Log(itemDescription);

        // 显示提示（如果有UIManager）
        // UIManager.Instance.ShowMessage(itemDescription);

        // 播放音效
        // AudioManager.Instance.PlaySound("ChestOpen");
    }

    private void GiveItemToPlayer()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("PlayerDataManager不存在！");
            return;
        }

        switch (itemType)
        {
            case ItemType.Bomb:
                PlayerDataManager.Instance.hasBomb = true;
                itemDescription = "获得了炸弹！可以炸开大门了";
                Debug.Log("【系统】玩家获得炸弹");
                break;

            case ItemType.HealthPotion:
                HealPlayer();
                break;

            case ItemType.Key:
                // 扩展用
                break;
        }
    }

    /// <summary>
    /// 恢复玩家生命值
    /// </summary>
    private void HealPlayer()
    {
        // 查找玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("找不到玩家！");
            return;
        }

        Character character = player.GetComponent<Character>();
        if (character == null)
        {
            Debug.LogError("玩家没有Character组件！");
            return;
        }

        // 检查是否满血
        if (character.currentHealth >= character.maxHealth)
        {
            itemDescription = "生命值已满，无法使用药水";
            Debug.Log("生命值已满");

            // 可以选择：不让宝箱打开，或者打开但不回血
            // 这里选择打开宝箱但不回血
            return;
        }

        // 恢复血量
        int oldHealth = character.currentHealth;
        character.currentHealth = Mathf.Min(character.currentHealth + healAmount, character.maxHealth);
        int actualHeal = character.currentHealth - oldHealth;

        // 同步到 PlayerDataManager
        PlayerDataManager.Instance.currentHealth = character.currentHealth;

        // 触发血量变化事件
        character.OnHealthChange?.Invoke(character);

        itemDescription = $"恢复了 {actualHeal} 点生命值！";
        Debug.Log($"【系统】玩家恢复 {actualHeal} 点血，当前血量: {character.currentHealth}/{character.maxHealth}");

        // 播放回血音效
        // AudioManager.Instance.PlaySound("Heal");
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR
        if (spriteRenderer != null && openSprite != null && closeSprite != null)
        {
            spriteRenderer.sprite = isDone ? openSprite : closeSprite;
        }
#endif
    }
}