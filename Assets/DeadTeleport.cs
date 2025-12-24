using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeadTeleport : MonoBehaviour
{
    public CanvasGroup blackScreen; // 拖入刚才创建的黑屏 UI
    public Transform targetPoint;  // 拖入一个空物体作为传送目的地
    public float fadeSpeed = 2f;    // 渐变速度

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 如果碰到的物体标签是 Thorns
        if (collision.gameObject.CompareTag("Thorns"))
        {
            Debug.Log("检测到碰撞");
            StartCoroutine(TeleportSequence());
        }
    }

    IEnumerator TeleportSequence()
    {
        Debug.Log("黑屏渐变开始！"); // 看看控制台有没有这行
        
        if (blackScreen == null) {
            Debug.LogError("未关联 Canvas Group！");
            yield break;
        }

        // 强制显示物体（以防万一你把它隐藏了）
        blackScreen.gameObject.SetActive(true);

        while (blackScreen.alpha < 1)
        {
            blackScreen.alpha += Time.deltaTime * fadeSpeed;
            Debug.Log("当前 Alpha: " + blackScreen.alpha); // 看看数值有没有变
            yield return null;
        }
        
        Debug.Log("黑屏完成，开始传送");
        transform.position = targetPoint.position;

        yield return new WaitForSeconds(0.5f);

        while (blackScreen.alpha > 0)
        {
            blackScreen.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}