using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CirclePuzzleTrigger : MonoBehaviour
{
    [Header("ID设置")]
    public int objectID;

    [Header("颜色设置")]
    public Color initialColor = Color.white;  // 初始颜色（白色）
    public Color triggeredColor = Color.black; // 触发后颜色（黑色）

    [Header("成功后的机关设置 (仅需在其中一个物体上配置)")]
    public GameObject targetObject; // 要移动/启用的物体（比如门）
    // 正确的触发顺序
    private static readonly int[] correctOrder = { 2, 1, 3 };
    
    // 共享状态（因为多个实例需要协同）
    private static List<int> currentInput = new List<int>();
    private static CirclePuzzleTrigger[] allTriggers;
    private static bool isSolved = false;

    private SpriteRenderer spriteRenderer;
    private bool hasBeenTriggered = false; // 标记自己是否已被触发

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 初始化为白色
            spriteRenderer.color = initialColor;
        }

        // 只在一个实例中初始化 allTriggers（避免重复）
        if (allTriggers == null || allTriggers.Length == 0)
        {
            allTriggers = FindObjectsOfType<CirclePuzzleTrigger>();
            // 确保所有物体初始为白色
            foreach (var trigger in allTriggers)
            {
                trigger.ResetVisual();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSolved || !other.CompareTag("Player") || hasBeenTriggered)
            return;

        // 标记自己已被触发，并变黑
        hasBeenTriggered = true;
        spriteRenderer.color = triggeredColor;

        // 记录输入
        currentInput.Add(objectID);
        //Debug.Log($"触碰 {objectID}，当前序列: {string.Join("-", currentInput)}");

        // 如果已输入3次，检查结果
        if (currentInput.Count == correctOrder.Length)
        {
            CheckPuzzle();
        }
    }

    /// <summary>
    /// 检查谜题是否成功
    /// </summary>
    private void CheckPuzzle()
    {
        if (currentInput.SequenceEqual(correctOrder))
        {
            //Debug.Log("<color=green>解谜成功！机关已开启。</color>");
            isSolved = true;

            // 启用目标物体（或移动）
            if (targetObject != null)
            {
                targetObject.SetActive(true); // 启用物体

                // 如果需要移动，可以在这里启动协程
                // StartCoroutine(MoveTarget(targetObject, targetPosition));
            }

            // 可选：禁用所有触发器
            foreach (var t in allTriggers)
            {
                t.enabled = false;
            }
        }
        else
        {
            //Debug.Log("<color=red>顺序错误，重置！</color>");
            ResetPuzzle();
        }
    }

    /// <summary>
    /// 重置整个谜题
    /// </summary>
    private void ResetPuzzle()
    {
        currentInput.Clear();
        foreach (var trigger in allTriggers)
        {
            trigger.ResetVisual();
        }
    }

    /// <summary>
    /// 重置单个物体的视觉状态
    /// </summary>
    private void ResetVisual()
    {
        hasBeenTriggered = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = initialColor;
        }
    }

}