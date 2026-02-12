using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 添加新输入系统命名空间
using TMPro;

public class TeachLevelManager : MonoBehaviour
{
    [Header("场景切换")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO nextScene;
    public Vector3 nextScenePosition;

    [Header("对话配置")]
    [TextArea(3, 5)]
    public List<string> dialogues = new List<string>();

    [Header("UI引用")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueHint;
    public TextMeshProUGUI nameText;

    [Header("打字机效果")]
    public bool useTypewriter = true;
    [Range(0.01f, 0.2f)]
    public float typeSpeed = 0.05f;
    public bool canSkipTyping = true;
    public AudioClip typeSound;
    [Range(0f, 1f)]
    public float typeSoundVolume = 0.5f;

    [Header("其他设置")]
    public string speakerName = "导师";

    // 新输入系统
    private PlayerInputControl inputControl;
    private bool submitPressed = false; // 标记是否按下

    private int currentDialogueIndex = -1;
    private bool isWaitingForInput = false;
    private bool isDialogueFinished = false;
    private bool isTyping = false;
    private Coroutine typewriterCoroutine;
    private AudioSource audioSource;

    void Awake()
    {
        // 初始化输入系统
        inputControl = new PlayerInputControl();
    }

    void OnEnable()
    {
        inputControl.Enable();

        // 绑定 Submit 动作（空格/回车/游戏键）
        inputControl.UI.Submit.performed += OnSubmitPerformed;

        // 绑定 Click 动作（鼠标左键）
        inputControl.UI.Click.performed += OnClickPerformed;
    }

    void OnDisable()
    {
        inputControl.UI.Submit.performed -= OnSubmitPerformed;
        inputControl.UI.Click.performed -= OnClickPerformed;
        inputControl.Disable();
    }

    void Start()
    {
        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.ResetAllData();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (typeSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = typeSoundVolume;
        }
        else if (GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 延迟开始
        Invoke(nameof(StartDialogue), 0.5f);
    }

    void Update()
    {
        // 检测标记
        if (submitPressed && isWaitingForInput && !isDialogueFinished)
        {
            submitPressed = false; // 重置标记
            HandleInput();
        }
    }

    private void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        if (isWaitingForInput && !isDialogueFinished)
        {
            submitPressed = true;
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (isWaitingForInput && !isDialogueFinished)
        {
            submitPressed = true;
        }
    }

    void HandleInput()
    {
        if (isTyping && canSkipTyping)
        {
            SkipTyping();
        }
        else if (!isTyping)
        {
            ShowNextDialogue();
        }
    }

    public void StartDialogue()
    {
        if (dialogues.Count == 0)
        {
            Debug.LogWarning("没有配置对话内容！");
            EndTeachLevel();
            return;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = speakerName;

        currentDialogueIndex = -1;
        isDialogueFinished = false;
        ShowNextDialogue();
    }

    void ShowNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogues.Count)
        {
            EndDialogue();
            return;
        }

        string currentText = dialogues[currentDialogueIndex];

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        if (useTypewriter)
        {
            typewriterCoroutine = StartCoroutine(TypeText(currentText));
        }
        else
        {
            dialogueText.text = currentText;
            isTyping = false;
        }

        bool isLast = (currentDialogueIndex == dialogues.Count - 1);
        if (continueHint != null)
        {
            continueHint.text = isLast ? "(点击结束)" : "(空格/点击继续)";
            continueHint.gameObject.SetActive(false);
        }

        isWaitingForInput = true;
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            if (typeSound != null && audioSource != null && fullText[i] != ' ')
            {
                audioSource.PlayOneShot(typeSound);
            }

            float waitTime = typeSpeed;
            if (IsPunctuation(fullText[i]))
            {
                waitTime *= 3f;
            }

            yield return new WaitForSeconds(waitTime);
        }

        isTyping = false;
        typewriterCoroutine = null;

        if (continueHint != null)
        {
            continueHint.gameObject.SetActive(true);
        }
    }

    void SkipTyping()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (currentDialogueIndex >= 0 && currentDialogueIndex < dialogues.Count)
        {
            dialogueText.text = dialogues[currentDialogueIndex];
        }

        isTyping = false;

        if (continueHint != null)
        {
            continueHint.gameObject.SetActive(true);
        }
    }

    bool IsPunctuation(char c)
    {
        return "，。！？、；：\"'（）【】".Contains(c.ToString())
            || ",.!?;:'\"()[]".Contains(c.ToString());
    }

    void EndDialogue()
    {
        isDialogueFinished = true;
        isWaitingForInput = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        Debug.Log("教学结束，进入第一关...");
        Invoke(nameof(EndTeachLevel), 1f);
    }

    void EndTeachLevel()
    {
        if (loadEventSO != null && nextScene != null)
        {
            loadEventSO.RaiseLoadRequestEvent(nextScene, nextScenePosition, true);
        }
        else
        {
            Debug.LogError("缺少场景切换配置！");
        }
    }

    // 调试跳过
    void OnGUI()
    {
        if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyDown)
        {
            EndTeachLevel();
        }
    }
}