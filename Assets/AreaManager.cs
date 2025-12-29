using UnityEngine;
using System.Collections.Generic;

public class AreaManager : MonoBehaviour
{
    [Header("--- 触发即禁用的物体清单 ---")]
    public string playerTag = "Player";
    public List<GameObject> objectsToDisable = new List<GameObject>(); 

    [Header("--- 声音与自动禁用设置 ---")]
    public GameObject targetObject; // 那个会发声的物体
    public float soundMaxDistance = 10f; // 声音完全消失的距离
    public float activeMaxDistance = 15f; // 物体彻底禁用的距离 (建议比声音距离大)

    private AudioSource audioSource;
    private Transform playerTransform;
    private bool isPlayerInZone = false;

    void Start()
    {
        if (targetObject != null)
        {
            audioSource = targetObject.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 只有当玩家进入过触发区，且目标物体存在时才执行
        if (isPlayerInZone && playerTransform != null && targetObject != null)
        {
            float distance = Vector2.Distance(playerTransform.position, targetObject.transform.position);

            // 1. 处理音量衰减
            if (audioSource != null)
            {
                // 计算音量：1 代表在物体位置，0 代表达到 soundMaxDistance
                float volume = 1 - (distance / soundMaxDistance);
                audioSource.volume = Mathf.Clamp01(volume);
            }

            // 2. 处理物体禁用/启用
            // 只有当玩家离得特别远 (超过 activeMaxDistance) 才禁用
            if (distance > activeMaxDistance)
            {
                if (targetObject.activeSelf) targetObject.SetActive(false);
            }
            else
            {
                // 如果玩家走回来了，重新激活它
                if (!targetObject.activeSelf) targetObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = true;
            playerTransform = other.transform;

            // 第一步：禁用列表里的物体（拖进去的那些）
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null) obj.SetActive(false);
            }

            // 第二步：确保那个发声物体在进入区域时是开启的
            if (targetObject != null) targetObject.SetActive(true);
        }
    }
}