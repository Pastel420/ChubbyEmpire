// RespawnPoint.cs
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("设置")]
    public bool setOnStart = true;  // 进入场景时自动设为复活点

    void Start()
    {
        if (setOnStart && PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.respawnPosition = transform.position;
            Debug.Log("复活点设置: " + transform.position);
        }
    }

    // 可视化
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
    }
}