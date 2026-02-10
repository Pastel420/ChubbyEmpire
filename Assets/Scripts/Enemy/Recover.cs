using UnityEngine;

public class Recover : MonoBehaviour
{
    [Header("恢复数值")]
    public int healAmount = 5;

    [Header("字幕设置")]
    public Color textColor = Color.red; // 👈 改成这个！鲜艳的红色
    public float floatSpeed = 1.5f;
    public float lifetime = 1.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是 Player 层
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Character playerCharacter = other.GetComponent<Character>();
            if (playerCharacter != null)
            {
                // 回复血量
                playerCharacter.currentHealth += healAmount;
                if (playerCharacter.currentHealth > playerCharacter.maxHealth)
                    playerCharacter.currentHealth = playerCharacter.maxHealth;

                playerCharacter.OnHealthChange?.Invoke(playerCharacter);
            }

            // 生成 "+5" 字幕
            SpawnHealText();

            // 销毁自己（药水/道具）
            Destroy(gameObject);
        }
    }

    private void SpawnHealText()
    {
        // 创建一个空 GameObject 作为字幕容器
        GameObject textObject = new GameObject("HealText");
        textObject.transform.position = transform.position + Vector3.up * 0.5f; // 稍微上移

        // 添加 TextMesh 组件（Unity 内置，无需 TMP）
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = "+" + healAmount;
        textMesh.fontSize = 16;
        textMesh.color = textColor;
        textMesh.anchor = TextAnchor.LowerCenter; // 底部居中，看起来从物品上浮起

        // 可选：调整字体（使用默认即可）
        // textMesh.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 添加一个简单脚本来控制上浮和淡出
        HealTextFader fader = textObject.AddComponent<HealTextFader>();
        fader.floatSpeed = floatSpeed;
        fader.lifetime = lifetime;
    }
}