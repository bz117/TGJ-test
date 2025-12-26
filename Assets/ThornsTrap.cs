using UnityEngine;
using System.Collections;

public class ThornsTrapWithSprite : MonoBehaviour
{
    [Header("传送设置")]
    public Transform targetPoint;  // 传送目的地
    public string playerTag = "Player";

    [Header("渐变设置")]
    public SpriteRenderer blackOverlay; // 拖入黑色 Sprite
    public float fadeSpeed = 1.5f;      // 渐变速度（越小越慢）
    public float waitAtBlack = 0.5f;    // 屏幕全黑时停留多久

    private bool isTeleporting = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 只有碰到玩家且不在传送中才触发
        if (collision.gameObject.CompareTag(playerTag) && !isTeleporting)
        {
            StartCoroutine(TeleportSequence(collision.transform));
        }
    }

    IEnumerator TeleportSequence(Transform playerTransform)
    {
        isTeleporting = true;
        
        if (blackOverlay == null) {
            Debug.LogError("请关联黑色 Sprite！");
            playerTransform.position = targetPoint.position;
            isTeleporting = false;
            yield break;
        }

        // --- 1. 逐渐变黑 (Fade In) ---
        Color c = blackOverlay.color;
        while (c.a < 1f)
        {
            // 使用 Mathf.MoveTowards 保证数值平滑增长
            c.a = Mathf.MoveTowards(c.a, 1f, Time.deltaTime * fadeSpeed);
            blackOverlay.color = c;
            yield return null; // 等待下一帧
        }
        
        // --- 2. 传送逻辑 ---
        // 此时屏幕全黑，玩家看不见，进行位置切换
        playerTransform.position = targetPoint.position;
        
        // 在全黑状态下稍微等一下，给玩家和相机一点反应时间
        yield return new WaitForSeconds(waitAtBlack);

        // --- 3. 逐渐变透明 (Fade Out) ---
        while (c.a > 0f)
        {
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * fadeSpeed);
            blackOverlay.color = c;
            yield return null; // 等待下一帧
        }

        isTeleporting = false;
    }
}