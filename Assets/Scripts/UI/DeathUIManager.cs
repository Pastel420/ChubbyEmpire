// DeathUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathUIManager : MonoBehaviour
{
    [Header("事件")]
    public PlayerDeathEventSO deathEvent;
    public SceneLoadEventSO sceneLoadedEvent;

    [Header("UI")]
    public GameObject deathPanel;
    public Button restartButton;            // 重新开始按钮
    public Button quitButton;               // 改为：退出游戏按钮
   

    private Vector3 respawnPosition;

    private void OnEnable()
    {
        deathEvent.OnPlayerDeath += ShowDeathUI;

        if (sceneLoadedEvent != null)
            sceneLoadedEvent.OnSceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        deathEvent.OnPlayerDeath -= ShowDeathUI;

        if (sceneLoadedEvent != null)
            sceneLoadedEvent.OnSceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartLevel);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitGame);  // 改为退出游戏
    }

    void ShowDeathUI()
    {
        if (PlayerDataManager.Instance != null)
        {
            respawnPosition = PlayerDataManager.Instance.respawnPosition;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);

        }

        Time.timeScale = 0f;
    }

    void OnSceneLoaded(GameSceneSO scene)
    {
        // 场景切换后隐藏死亡面板
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // 确保时间恢复
        Time.timeScale = 1f;
    }

    public void OnRestartLevel()
    {
        // 先恢复时间
        Time.timeScale = 1f;

        // 隐藏面板
        if (deathPanel != null)
            deathPanel.SetActive(false);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var character = player.GetComponent<Character>();
            if (character != null)
            {
                character.currentHealth = character.maxHealth;

                if (PlayerDataManager.Instance != null)
                {
                    PlayerDataManager.Instance.currentHealth = character.maxHealth;
                }

                character.OnHealthChange?.Invoke(character);
            }

            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.isDead = false;
                controller.isHurt = false;
                controller.inputControl.Gameplay.Enable();
            }

            var animator = player.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }

            player.transform.position = respawnPosition;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }
        }

        Debug.Log("重新开始本关");
    }

    /// <summary>
    /// 退出游戏（替代返回菜单）
    /// </summary>
    public void OnQuitGame()
    {
        // 恢复时间
        Time.timeScale = 1f;

        Debug.Log("退出游戏");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}