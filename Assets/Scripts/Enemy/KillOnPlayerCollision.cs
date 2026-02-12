
using UnityEngine;

public class KillOnPlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是 Player 层
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                character.Die(); // 👈 直接调用死亡方法
                Debug.Log("Player killed instantly by trigger!");
            }
            else
            {
                Debug.LogWarning("Player missing Character component!");
            }
        }
    }
}