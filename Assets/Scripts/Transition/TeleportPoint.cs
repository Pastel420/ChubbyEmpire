using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, Iinteractable
{
    [Header("场景加载")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;

    [Header("传送条件")]
    public bool requireCondition = false;       // 是否需要满足条件
    public ConditionType conditionType;         // 条件类型
    public string failMessage = "需要炸弹开门"; // 不满足条件时的提示

    [Header("音效")]
    public AudioClip teleportSound;
    public enum ConditionType
    {
        None,
        HasBomb,           // 需要炸弹（P、S关卡）
        BossDefeated       // 需要击败Boss（C关卡）
    }

    public void TriggerAction()
    {
        // 检查条件
        if (requireCondition && !CheckCondition())
        {
            //Debug.Log(failMessage);
            // 这里可以调用UIManager显示提示
            // UIManager.Instance.ShowMessage(failMessage);
            return;
        }
        // 播放传送音效
        if (teleportSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(teleportSound);
        }

        //Debug.Log("传送到: " + sceneToGo.name);
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }

    private bool CheckCondition()
    {
        switch (conditionType)
        {
            case ConditionType.HasBomb:
                return PlayerDataManager.Instance.hasBomb;
            case ConditionType.BossDefeated:
                return PlayerDataManager.Instance.bossDefeated;
            default:
                return true;
        }
    }

    // 可视化范围
    private void OnDrawGizmos()
    {
        Gizmos.color = requireCondition ? Color.yellow : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}