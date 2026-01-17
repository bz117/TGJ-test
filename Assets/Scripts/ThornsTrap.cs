using UnityEngine;
using System.Collections;
using UnityEngine.UI; // 必须引入 UI 命名空间

public class ThornsTrapWithCanvasFade : MonoBehaviour
{
    [Header("传送设置")]
    public Transform targetPoint;           // 传送目的地
    public string playerTag = "Player";

    [Header("渐变设置")]
    public Image blackOverlayImage;         // 拖入 Canvas 下的 Image（全屏黑色）
    public float fadeSpeed = 1.5f;          // 渐变速度（越大越快）
    public float waitAtBlack = 0.5f;        // 屏幕全黑时停留多久

    private bool isTeleporting = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && !isTeleporting)
        {
            StartCoroutine(TeleportSequence(collision.transform));
        }
    }

    IEnumerator TeleportSequence(Transform playerTransform)
    {
        isTeleporting = true;

        // 检查是否关联了 Image
        if (blackOverlayImage == null)
        {
            Debug.LogError("请在 Inspector 中指定 Black Overlay Image！");
            playerTransform.position = targetPoint.position;
            isTeleporting = false;
            yield break;
        }

        // 确保 Image 所在的 Canvas 是激活的
        Canvas canvas = blackOverlayImage.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(true);
        }

        Color c = blackOverlayImage.color;

        // --- 1. 淡入（变黑）---
        while (c.a < 1f)
        {
            c.a = Mathf.MoveTowards(c.a, 1f, Time.deltaTime * fadeSpeed);
            blackOverlayImage.color = c;
            yield return null;
        }

        // --- 2. 传送玩家 ---
        playerTransform.position = targetPoint.position;
        yield return new WaitForSeconds(waitAtBlack);

        // --- 3. 淡出（变透明）---
        while (c.a > 0f)
        {
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * fadeSpeed);
            blackOverlayImage.color = c;
            yield return null;
        }

        // 可选：传送完成后隐藏 Canvas（节省性能）
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }

        isTeleporting = false;
    }
}