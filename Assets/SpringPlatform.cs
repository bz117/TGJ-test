using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    [Header("设置参数")]
    public float bounceForce = 20f;
    public Color usedColor = Color.red;
    
    [Header("碰撞体缩放设置")]
    public Vector2 deactivatedSize = new Vector2(1f, 0.5f);

    private bool isReady = true;
    
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Vector2 originalSize;
    private Vector2 originalOffset;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // 记录原始值，用于计算偏移量
        originalSize = boxCollider.size;
        originalOffset = boxCollider.offset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果已经触发过，则不再执行弹起逻辑
        if (isReady && collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 检测是否是从上方踩踏
                if (contact.normal.y < -0.5f) 
                {
                    ActivateSpring(collision.gameObject);
                    break;
                }
            }
        }
    }

    private void ActivateSpring(GameObject player)
    {
        isReady = false; // 标记为已触发，后续不再生效

        // 1. 给玩家向上的力
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 使用 velocity 直接赋值可以确保弹跳高度一致
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        }

        // 2. 视觉反馈：改变颜色
        spriteRenderer.color = usedColor;

        // 3. 永久改变碰撞体大小
        UpdateCollider(deactivatedSize);
    }

    private void UpdateCollider(Vector2 newSize)
    {
        if (boxCollider == null) return;
        
        // 计算高度差，使碰撞体底部对齐（防止平台悬空）
        float heightDifference = (originalSize.y - newSize.y) / 2f;
        boxCollider.size = newSize;
        boxCollider.offset = new Vector2(originalOffset.x, originalOffset.y - heightDifference);
    }
}