using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class EndLevelManager : MonoBehaviour, Iinteractable
{
    [Header("公主对话")]
    [TextArea]
    public List<string> princessDialogues = new List<string>
    {
        "勇士，你终于来了！",
        "谢谢你击败魔王，拯救了王国。",
        "你的勇气将被永远传颂。",
        "现在，你可以休息了吗？"
    };

    public string princessName = "公主";

    [Header("场景切换")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO startScene;

    [Header("UI引用")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueHint;

    [Header("选择面板")]
    public GameObject choicePanel;
    public GameObject endGameButton;
    public GameObject returnButton;

    [Header("打字机效果")]
    public bool useTypewriter = true;
    public float typeSpeed = 0.05f;

    private int currentDialogueIndex = -1;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typewriterCoroutine;

    private PlayerInputControl inputControl;
    private PlayerController playerController; // 新增：玩家控制器

    void Awake()
    {
        inputControl = new PlayerInputControl();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    void OnEnable()
    {
        inputControl.Enable();
        inputControl.UI.Submit.performed += OnSubmitPerformed;
    }

    void OnDisable()
    {
        inputControl.UI.Submit.performed -= OnSubmitPerformed;
        inputControl.Disable();

        // 确保恢复玩家输入
        EnablePlayerInput();
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        if (isDialogueActive && !choicePanel.activeSelf)
        {
            HandleDialogueInput();
        }
    }

    public void TriggerAction()
    {
        if (!isDialogueActive)
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        currentDialogueIndex = -1;

        // 禁用玩家输入
        DisablePlayerInput();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = princessName;

        if (choicePanel != null)
            choicePanel.SetActive(false);

        ShowNextDialogue();
    }

    /// <summary>
    /// 禁用玩家Gameplay输入
    /// </summary>
    void DisablePlayerInput()
    {
        // 查找玩家
        if (playerController == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerController>();
            }
        }

        // 禁用Gameplay，启用UI
        if (playerController != null && playerController.inputControl != null)
        {
            playerController.inputControl.Gameplay.Disable();
            Debug.Log("玩家Gameplay输入已禁用");
        }
    }

    /// <summary>
    /// 恢复玩家Gameplay输入
    /// </summary>
    void EnablePlayerInput()
    {
        if (playerController != null && playerController.inputControl != null)
        {
            // 确保玩家不在死亡状态才恢复输入
            if (!playerController.isDead)
            {
                playerController.inputControl.Gameplay.Enable();
                Debug.Log("玩家Gameplay输入已恢复");
            }
        }
    }

    void HandleDialogueInput()
    {
        if (isTyping && useTypewriter)
        {
            SkipTyping();
        }
        else
        {
            ShowNextDialogue();
        }
    }

    void ShowNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= princessDialogues.Count)
        {
            EndDialogue();
            return;
        }

        string currentText = princessDialogues[currentDialogueIndex];

        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        if (useTypewriter)
        {
            typewriterCoroutine = StartCoroutine(TypeText(currentText));
        }
        else
        {
            dialogueText.text = currentText;
            isTyping = false;
        }

        bool isLast = (currentDialogueIndex == princessDialogues.Count - 1);
        if (continueHint != null)
        {
            continueHint.text = isLast ? "(按E结束对话)" : "(按E继续)";
            continueHint.gameObject.SetActive(!useTypewriter || !isTyping);
        }
    }

    System.Collections.IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        if (continueHint != null)
            continueHint.gameObject.SetActive(false);

        foreach (char c in fullText)
        {
            dialogueText.text += c;

            float waitTime = typeSpeed;
            if (IsPunctuation(c))
                waitTime *= 3f;

            yield return new WaitForSeconds(waitTime);
        }

        isTyping = false;

        if (continueHint != null)
            continueHint.gameObject.SetActive(true);
    }

    void SkipTyping()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (currentDialogueIndex >= 0 && currentDialogueIndex < princessDialogues.Count)
        {
            dialogueText.text = princessDialogues[currentDialogueIndex];
        }

        isTyping = false;

        if (continueHint != null)
            continueHint.gameObject.SetActive(true);
    }

    void EndDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    // 按钮调用：结束游戏
    public void OnEndGame()
    {
        EnablePlayerInput(); // 恢复输入（虽然要退出了）

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // 按钮调用：返回标题
    public void OnReturnToTitle()
    {
        // 恢复输入（虽然要切换场景了）
        EnablePlayerInput();

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.ResetAllData();
        }

        if (loadEventSO != null && startScene != null)
        {
            loadEventSO.RaiseLoadRequestEvent(startScene, Vector3.zero, true);
        }
    }

    bool IsPunctuation(char c)
    {
        return "，。！？、；：\"'（）【】".Contains(c.ToString())
            || ",.!?;:'\"()[]".Contains(c.ToString());
    }
}