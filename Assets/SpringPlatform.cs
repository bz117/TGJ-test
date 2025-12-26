using UnityEngine;
using System.Collections;

public class SpringPlatform : MonoBehaviour
{
    [Header("设置参数")]
    public float bounceForce = 2000f;      // 弹跳力度
    public float recoverTime = 5f;       // 恢复时间
    public Color usedColor = Color.red;  // 变色颜色

    private Color originalColor;         // 记录初始颜色
    private bool isReady = true;         // 是否处于可弹跳状态
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        Debug.Log("脚本运行成功");
    }

    // 2D 碰撞检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("检测到碰撞");
        // 1. 判断是否处于激活状态
        // 2. 判断碰撞的是否是玩家（假设玩家有 "Player" 标签）
        if (isReady && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("检测到玩家碰撞");
            // 3. 判断玩家是否是从上方跳下来的
            // 获取碰撞接触点，判断法线方向（向上）
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f) // 法线向下说明物体被从上方踩踏
                {
                    ActivateSpring(collision.gameObject);
                    break;
                }
            }
        }
    }

    private void ActivateSpring(GameObject player)
    {
        isReady = false; // 禁用弹力
        Debug.Log("禁用弹力成功");

        // 让玩家弹起
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 重置 Y 轴速度，确保弹跳高度一致
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        }

        // 改变物体颜色
        spriteRenderer.color = usedColor;

        // 开启协程，5秒后恢复
        StartCoroutine(RecoverRoutine());
    }

    IEnumerator RecoverRoutine()
    {
        yield return new WaitForSeconds(recoverTime);
        
        // 恢复初始状态
        spriteRenderer.color = originalColor;
        isReady = true;
    }
}