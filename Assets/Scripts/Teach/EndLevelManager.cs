using UnityEngine;
using System.Collections.Generic;

public class EndLevelManager : MonoBehaviour, Iinteractable
{
    [Header("公主对话")]
    [TextArea]
    public List<string> princessDialogues = new List<string>();

    [Header("场景")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO startScene;          // Start场景

    [Header("UI")]
    // public GameObject choicePanel;        // 选择面板（结束游戏/返回标题）

    private int currentDialogueIndex = -1;
    private bool isDialogueActive = false;
    private bool canMakeChoice = false;

    void Update()
    {
        if (isDialogueActive && !canMakeChoice && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextPrincessDialogue();
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
        ShowNextPrincessDialogue();
    }

    void ShowNextPrincessDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex >= princessDialogues.Count)
        {
            // 对话结束，显示选择
            ShowChoices();
            return;
        }

        // 更新UI
        // dialogueText.text = princessDialogues[currentDialogueIndex];
        Debug.Log("公主: " + princessDialogues[currentDialogueIndex]);
    }

    void ShowChoices()
    {
        canMakeChoice = true;
        // choicePanel.SetActive(true);
        Debug.Log("1. 结束游戏  2. 返回标题");
    }

    // 按钮调用
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // 按钮调用
    public void OnReturnToTitle()
    {
        PlayerDataManager.Instance.ResetAllData();
        loadEventSO.RaiseLoadRequestEvent(startScene, Vector3.zero, true);
    }
}