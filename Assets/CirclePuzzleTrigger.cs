using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("ID设置")]
    public int objectID;

    [Header("闪烁设置")]
    public Color flashColor = Color.white;
    public float flashDuration = 1.0f;

    [Header("成功后的机关设置 (仅需在其中一个物体上配置)")]
    public GameObject targetObject;    // 要移动的物体（比如门）
    public Vector3 targetPosition;     // 目标坐标
    public float moveSpeed = 2f;       // 移动速度

    private static readonly int[] correctOrder = { 1, 2, 3 }; 
    private static List<int> currentInput = new List<int>();
    private static bool isSolved = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSolved || !collision.CompareTag("Player")) return;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DoFlash());

        currentInput.Add(objectID);
        Debug.Log($"触碰 {objectID}，当前序列: {string.Join("-", currentInput)}");

        if (currentInput.Count == 3) CheckPuzzle();
    }

    private IEnumerator DoFlash()
    {
        float halfDuration = flashDuration / 2f;
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(originalColor, flashColor, elapsed / halfDuration);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(flashColor, originalColor, elapsed / halfDuration);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }

    private void CheckPuzzle()
    {
        if (currentInput.SequenceEqual(correctOrder))
        {
            Debug.Log("<color=green>解谜成功！机关已开启。</color>");
            isSolved = true;
            
            // 找到所有触发器并禁用，同时启动移动逻辑
            PuzzleTrigger[] allTriggers = FindObjectsOfType<PuzzleTrigger>();
            foreach (var t in allTriggers)
            {
                // 如果这个脚本实例配置了目标物体，就启动它的移动协程
                if (t.targetObject != null)
                {
                    t.StartCoroutine(MoveTarget(t.targetObject, t.targetPosition));
                }
                t.enabled = false;
            }
        }
        else
        {
            Debug.Log("<color=red>顺序错误，重置！</color>");
            currentInput.Clear();
        }
    }

    private Vector3 velocity = Vector3.zero; // 必须定义在循环外

    private IEnumerator MoveTarget(GameObject obj, Vector3 targetPos)
    {
        float smoothTime = 3.0f; // 无论距离多远，大约 3.0 秒到达
        
        while (Vector3.Distance(obj.transform.position, targetPos) > 0.01f)
        {
            obj.transform.position = Vector3.SmoothDamp(
                obj.transform.position, 
                targetPos, 
                ref velocity, 
                smoothTime
            );
            yield return null;
        }
        obj.transform.position = targetPos;
    }
}