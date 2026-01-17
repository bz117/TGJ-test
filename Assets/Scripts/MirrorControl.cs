using UnityEngine;
using System.Collections;

public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;

    [Header("触发设置")]
    public int totalRequired = 3;
    private int currentCount = 0;

    [Header("目标物体")]
    public SpriteRenderer fadeTarget; 
    public Transform moveTarget;

    [Header("相机设置")]
    public Camera mainCamera;
    public Transform cameraFocusPoint;
    public float cameraMoveSpeed = 1.5f;

    [Header("动画参数")]
    public float fadeDuration = 2.0f;
    public Vector3 targetMovePosition;
    public float moveDuration = 2.0f;
    
    [Header("停顿时间")]
    public float postEffectDelay = 1.5f;

    private void Awake()
    {
        Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
        
        if (fadeTarget != null) {
            Color c = fadeTarget.color;
            c.a = 0f;
            fadeTarget.color = c;
        }
    }

    public void NotifyTriggered()
    {
        currentCount++;
        if (currentCount >= totalRequired)
        {
            StartCoroutine(ExecuteFinalSequence());
        }
    }

    IEnumerator ExecuteFinalSequence()
    {
        CameraFollow followScript = mainCamera.GetComponent<CameraFollow>();
        if (followScript != null) followScript.enabled = false; 

        // 1. 移动镜头
        Vector3 originalCamPos = mainCamera.transform.position;
        Vector3 targetCamPos = new Vector3(cameraFocusPoint.position.x, cameraFocusPoint.position.y, originalCamPos.z);
        yield return StartCoroutine(MoveCamera(mainCamera.transform, targetCamPos));

        // 2. 执行物体特效 (现在按顺序执行)
        Debug.Log("2a. 开始淡入...");
        yield return StartCoroutine(FadeInEffect());
        
        Debug.Log("2b. 开始移动...");
        yield return StartCoroutine(MoveEffect());

        // 3. 停留
        yield return new WaitForSeconds(postEffectDelay);

        // 4. 镜头回切
        yield return StartCoroutine(MoveCamera(mainCamera.transform, originalCamPos));
        
        if (followScript != null) followScript.enabled = true; 
    }

    // --- 拆分后的逻辑函数 ---

    IEnumerator FadeInEffect()
    {
        if (fadeTarget == null) yield break;

        float elapsed = 0f;
        Color startColor = fadeTarget.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeTarget.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        fadeTarget.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }

    IEnumerator MoveEffect()
    {
        if (moveTarget == null) yield break;

        float elapsed = 0f;
        Vector3 startPos = moveTarget.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.SmoothStep(0, 1, elapsed / moveDuration); // 使用平滑插值，手感更好
            moveTarget.position = Vector3.Lerp(startPos, targetMovePosition, percent);
            yield return null;
        }
        moveTarget.position = targetMovePosition;
    }

    IEnumerator MoveCamera(Transform cam, Vector3 target)
    {
        float elapsed = 0;
        Vector3 start = cam.position;
        while (elapsed < cameraMoveSpeed)
        {
            elapsed += Time.deltaTime;
            cam.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0, 1, elapsed / cameraMoveSpeed));
            yield return null;
        }
        cam.position = target;
    }
}