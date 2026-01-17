using UnityEngine;
using System.Collections;

public class SingleFlashZoneEffect : MonoBehaviour
{
    [Header("时间与速度")]
    public float pauseDuration = 3.0f;    // 玩家被定身的总时长
    public float flashInSpeed = 4.0f;     // 变白的速度
    public float flashOutSpeed = 2.0f;    // 恢复原色的速度

    [Header("引用 (不填则自动获取)")]
    public GameObject targetObject;       // 要闪烁的物体
    public AudioSource audioSource;       // 音乐组件

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isEffectRunning = false;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        if (targetObject != null)
        {
            sr = targetObject.GetComponent<SpriteRenderer>();
            originalColor = sr.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查标签且确保当前没有正在执行效果
        if (other.CompareTag("Player") && !isEffectRunning)
        {
            StartCoroutine(SingleFlashSequence(other.gameObject));
        }
    }

    IEnumerator SingleFlashSequence(GameObject player)
    {
        isEffectRunning = true;

        // --- 1. 禁用操作 (与之前相同) ---
        var controllers = player.GetComponents<MonoBehaviour>();
        foreach (var c in controllers) { if (c != this) c.enabled = false; }
        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // --- 3. 执行“只闪烁一次”的渐变逻辑 ---
        if (sr != null)
        {
            // A. 渐变到白色
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * flashInSpeed;
                sr.color = Color.Lerp(originalColor, Color.white, t);
                yield return null;
            }

            // B. 渐变回原色
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * flashOutSpeed;
                sr.color = Color.Lerp(Color.white, originalColor, t);
                yield return null;
            }
        }
        // --- 2. 播放音乐 ---
        if (audioSource != null) audioSource.Play();

        // --- 4. 等待剩余的暂停时间 ---
        // (总时长减去闪烁消耗的时间，如果想让闪烁完立即恢复，这里可以删掉)
        yield return new WaitForSeconds(pauseDuration); 

        // --- 5. 恢复状态 ---
        if (audioSource != null) audioSource.Stop();
        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        foreach (var c in controllers) { if (c != null) c.enabled = true; }

        // 彻底关闭触发器，它再也不会响应任何碰撞
        GetComponent<Collider2D>().enabled = false;
        
        isEffectRunning = false;

        // 在 SingleFlashSequence 协程的最末尾（isEffectRunning = false 之前）添加：
        if (TriggerManager.Instance != null)
        {
            TriggerManager.Instance.NotifyTriggered();
        }
    }
}