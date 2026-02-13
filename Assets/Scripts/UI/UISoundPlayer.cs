// UISoundPlayer.cs
using UnityEngine;
using UnityEngine.UI;

public class UISoundPlayer : MonoBehaviour
{
    [Header("UI音效")]
    public AudioClip buttonClickSound;      // 按钮点击音效
    public AudioClip buttonHoverSound;      // 按钮悬停音效（可选）

    [Header("交互音效")]
    public AudioClip interactSuccessSound;  // 交互成功
    public AudioClip interactFailSound;     // 交互失败（如条件不满足）

    void Start()
    {
        // 自动给所有按钮添加音效
        AutoSetupButtons();
    }

    /// <summary>
    /// 自动查找场景中的所有按钮并添加音效
    /// </summary>
    void AutoSetupButtons()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 包括禁用的按钮

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() => PlayButtonClick());

            // 可选：悬停音效
            // var trigger = btn.gameObject.AddComponent<EventTrigger>();
            // 添加PointerEnter事件...
        }

        Debug.Log($"已为 {buttons.Length} 个按钮添加点击音效");
    }

    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    public void PlayButtonClick()
    {
        if (AudioManager.Instance != null && buttonClickSound != null)
        {
            AudioManager.Instance.PlaySFX(buttonClickSound);
        }
    }

    /// <summary>
    /// 播放交互成功音效
    /// </summary>
    public void PlayInteractSuccess()
    {
        if (AudioManager.Instance != null && interactSuccessSound != null)
        {
            AudioManager.Instance.PlaySFX(interactSuccessSound);
        }
    }

    /// <summary>
    /// 播放交互失败音效
    /// </summary>
    public void PlayInteractFail()
    {
        if (AudioManager.Instance != null && interactFailSound != null)
        {
            AudioManager.Instance.PlaySFX(interactFailSound);
        }
    }
}