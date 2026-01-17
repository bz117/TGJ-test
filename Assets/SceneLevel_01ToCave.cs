using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    [Header("跳转设置")]
    public string targetSceneName;
    
    [Header("引用设置")]
    public SpriteRenderer fadeOverlay; 
    public float fadeDuration = 1.0f;
    public float waitBeforeFadeIn = 2.0f; // 等待时间

    [Header("输入限制")]
    [Tooltip("拖入玩家身上负责移动的脚本组件")]
    public MonoBehaviour playerControlScript; 

    private static bool shouldFadeIn = false; 
    private bool isTransitioning = false;

    private void Start()
    {
        // 自动尝试寻找玩家脚本（如果面板没拖的话）
        if (playerControlScript == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // 这里需要根据你实际的移动脚本名称修改，例如 PlayerController
                // 如果你有多个脚本，建议通过 Tag 找到玩家后手动赋值
                // playerControlScript = player.GetComponent<YourMovementScript>();
            }
        }

        if (shouldFadeIn && fadeOverlay != null)
        {
            StartCoroutine(FadeFromBlack());
        }
        else if (fadeOverlay != null)
        {
            SetAlpha(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning || !collision.CompareTag("Player")) return;

        // 触发瞬间禁用操作
        TogglePlayerControl(false);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            StartCoroutine(FadeToBlackAndExit());
        }
    }

    private IEnumerator FadeToBlackAndExit()
    {
        isTransitioning = true;
        yield return StartCoroutine(UpdateAlpha(0, 1));
        shouldFadeIn = true;
        SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator FadeFromBlack()
    {
        isTransitioning = true;
        
        // 初始确保全黑并禁用操作
        SetAlpha(1);
        TogglePlayerControl(false);

        yield return new WaitForSeconds(waitBeforeFadeIn); 

        yield return StartCoroutine(UpdateAlpha(1, 0));
        
        // 渐亮完成后恢复操作
        TogglePlayerControl(true);
        
        shouldFadeIn = false;
        isTransitioning = false;
    }

    // 控制玩家脚本启用的开关
    private void TogglePlayerControl(bool state)
    {
        if (playerControlScript != null)
        {
            playerControlScript.enabled = state;
            Debug.Log(state ? "玩家输入已恢复" : "玩家输入已禁用");
        }
    }

    private IEnumerator UpdateAlpha(float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(start, end, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(end);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = alpha;
            fadeOverlay.color = c;
        }
    }
}