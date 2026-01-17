using UnityEngine;

public class DisableOnPlayerTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入触发器的物体是否是 Player
        if (other.CompareTag("Player"))
        {
            // 禁用当前 GameObject（包括所有组件和子物体）
            gameObject.SetActive(false);
        }
    }
}