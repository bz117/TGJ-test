using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("传送设置")]
    public Transform targetPoint;  // 传送目的地
    public string playerTag = "Player";

    // 当物体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查触发对象的标签是否为玩家
        if (other.CompareTag(playerTag))
        {
            Teleport(other.transform);
        }
    }

    private void Teleport(Transform playerTransform)
    {
        if (targetPoint != null)
        {
            // 直接将玩家位置设为目的地位置
            playerTransform.position = targetPoint.position;
        }
        else
        {
            Debug.LogWarning("未设置传送目的地 (targetPoint)！");
        }
    }
}