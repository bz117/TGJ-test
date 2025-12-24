using UnityEngine;

public class SetParent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 关键点 1：立刻清除所有速度，防止向下冲击力造成挤压
                rb.velocity = Vector2.zero;
                
                // 关键点 2：将重力改为 0 之前，稍微把玩家向上位移一点点（例如 0.05米）
                // 这样可以确保玩家的 Collider 离开平台的内部，避免物理排斥
                collision.transform.position += new Vector3(0, 0.05f, 0);

                rb.gravityScale = 0f;
            }

            collision.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 离开时记得恢复碰撞
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), false);

            collision.transform.SetParent(null);
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null) rb.gravityScale = 1f;
        }
    }
}