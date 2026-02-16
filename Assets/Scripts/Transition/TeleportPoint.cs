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
    public bool requireCondition = false;
    public ConditionType conditionType;
    public string failMessage = "需要炸弹开门";

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
            // UIManager.Instance.ShowMessage(failMessage);
            return;
        }

        // ✅ 关键：传送时清空炸弹状态（防止带入下一关）
        PlayerDataManager.Instance.hasBomb = false;

        // 播放传送音效
        if (teleportSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(teleportSound);
        }

        // 加载目标场景
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

    private void OnDrawGizmos()
    {
        Gizmos.color = requireCondition ? Color.yellow : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}