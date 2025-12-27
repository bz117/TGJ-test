using UnityEngine;
using TMPro; // 如果使用 TextMeshPro
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [Header("UI 组件")]
    public CanvasGroup uiCanvasGroup; // 拖入教学文字的 Canvas Group

    [Header("时间调节 (秒)")]
    [Range(0.1f, 5f)] 
    public float fadeInTime = 1.5f;   // 淡入持续时间
    
    public float displayTime = 10.0f;  // 文字停留时间
    
    [Range(0.1f, 5f)]
    public float fadeOutTime = 2.0f;  // 淡出持续时间

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家触发
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(TutorialSequence());
        }
    }

    IEnumerator TutorialSequence()
    {
        // 1. 淡入
        yield return StartCoroutine(FadeCanvas(0, 1, fadeInTime));

        // 2. 停留
        yield return new WaitForSeconds(displayTime);

        // 3. 淡出
        yield return StartCoroutine(FadeCanvas(1, 0, fadeOutTime));

        // 4. 禁用脚本（防止再次触发）
        this.enabled = false;
    }

    // 通用的淡入淡出逻辑
    IEnumerator FadeCanvas(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            uiCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        uiCanvasGroup.alpha = endAlpha;
    }
}