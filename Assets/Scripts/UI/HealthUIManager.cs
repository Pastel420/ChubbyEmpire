// HealthUIManager.cs 修复版
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HealthUIManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject playerStatBar;
    public List<Image> hearts = new List<Image>();

    [Header("事件")]
    public SceneLoadEventSO sceneLoadedEvent;

    private Character playerCharacter;
    private int maxHealth = 5;
    private bool isInGameScene = false;

    void Awake()
    {
        if (playerStatBar != null)
            playerStatBar.SetActive(false);
    }

    void OnEnable()
    {
        if (sceneLoadedEvent != null)
            sceneLoadedEvent.OnSceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (sceneLoadedEvent != null)
            sceneLoadedEvent.OnSceneLoaded -= OnSceneLoaded;

        if (playerCharacter != null)
            playerCharacter.OnHealthChange.RemoveListener(UpdateHealthUI);
    }

    void OnSceneLoaded(GameSceneSO scene)
    {
        // 清理旧绑定
        if (playerCharacter != null)
        {
            playerCharacter.OnHealthChange.RemoveListener(UpdateHealthUI);
            playerCharacter = null;
        }

        bool isGameScene = scene.sceneType == GameSceneSO.SceneType.Level
                        || scene.sceneType == GameSceneSO.SceneType.Boss
                        || scene.sceneType == GameSceneSO.SceneType.End;

        isInGameScene = isGameScene;

        if (isGameScene)
        {
            if (playerStatBar != null)
                playerStatBar.SetActive(true);

            // 延迟查找玩家，等待SceneLoader激活玩家
            StartCoroutine(FindPlayerDelayed());
        }
        else
        {
            if (playerStatBar != null)
                playerStatBar.SetActive(false);
            isInGameScene = false;
        }
    }

    System.Collections.IEnumerator FindPlayerDelayed()
    {
        // 等待多帧，确保SceneLoader激活了玩家
        for (int i = 0; i < 10; i++) // 最多等待10帧
        {
            yield return null;

            var player = GameObject.FindWithTag("Player");
            if (player != null && player.activeInHierarchy)
            {
                SetupPlayer(player);
                yield break; // 找到就退出
            }
        }

        Debug.LogError("等待10帧后仍未找到激活的玩家！");
    }

    void SetupPlayer(GameObject player)
    {
        Debug.Log($"找到玩家: {player.name}, 激活状态: {player.activeInHierarchy}");

        playerCharacter = player.GetComponent<Character>();
        if (playerCharacter == null)
        {
            Debug.LogError("玩家没有Character组件！");
            return;
        }

        // 绑定事件
        playerCharacter.OnHealthChange.AddListener(UpdateHealthUI);
        Debug.Log("已绑定血量变化事件");

        maxHealth = playerCharacter.maxHealth;

        // 初始更新显示
        UpdateHealthUI(playerCharacter);
    }

    public void UpdateHealthUI(Character character)
    {
        int currentHealth = character.currentHealth;
        Debug.Log($"更新血量UI: 当前血量 {currentHealth}");

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] != null)
            {
                bool shouldShow = i < currentHealth;
                hearts[i].gameObject.SetActive(shouldShow);
            }
        }
    }

    // 测试按键
    void Update()
    {
        if (!isInGameScene || playerCharacter == null) return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            Debug.Log("按T键测试扣血");
            playerCharacter.currentHealth = Mathf.Max(0, playerCharacter.currentHealth - 1);

            // 手动触发事件（如果事件没绑定成功）
            UpdateHealthUI(playerCharacter);
        }

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            Debug.Log("按Y键测试回血");
            playerCharacter.currentHealth = Mathf.Min(maxHealth, playerCharacter.currentHealth + 1);
            UpdateHealthUI(playerCharacter);
        }
    }
}