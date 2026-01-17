using UnityEngine;

public class EnableDoubleJumpOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是 Player
        if (other.CompareTag("Player"))
        {
            // 尝试获取 Player 身上的 PlayerMovement 组件
            PlayerController playerMovement = other.GetComponent<PlayerController>();
            
            if (playerMovement != null)
            {
                playerMovement.canDoubleJump = true;
                Debug.Log("Double jump enabled!");
            }
            else
            {
                Debug.LogWarning("Player does not have a PlayerMovement component!");
            }
            gameObject.SetActive(false); // 或 Destroy(gameObject);
        }
    }
}