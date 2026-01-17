using UnityEngine;

/// <summary>
/// 鸟群协调器：管理所有小鸟的激活与到达状态，并在全部到达后触发最终事件。
/// 挂载在鸟群的父物体（如 BirdsFlock）上。
/// </summary>
public class BirdCoordinator : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("需要等待多少只小鸟全部到达")]
    public int totalBirds = 4; // 通常为4

    [Header("最终事件")]
    [Tooltip("所有小鸟到达后要激活的物体，例如一座桥")]
    public GameObject objectToActivateOnComplete;

    [Header("可选：玩家控制")]
    [Tooltip("是否在事件完成后解锁玩家移动？")]
    public bool unlockPlayerOnComplete = true;
    public string playerTag = "Player";

    // 内部状态
    private int _arrivedBirdCount = 0;
    private bool _hasCompleted = false;

    void Start()
    {
        // 初始化计数
        _arrivedBirdCount = 0;
        _hasCompleted = false;

        // 可选：游戏开始时禁用要激活的物体
        if (objectToActivateOnComplete != null)
        {
            objectToActivateOnComplete.SetActive(false);
        }
    }

    /// <summary>
    /// 供 BirdTrigger 调用，注册自己已被声波激活。
    /// （此方法目前仅用于调试或未来扩展，核心逻辑在 OnBirdArrived）
    /// </summary>
    public void RegisterSender(BirdTrigger sender)
    {
        // 这里可以添加日志或未来逻辑
        Debug.Log($"小鸟 {sender.name} 已被激活。");
    }

    /// <summary>
    /// 核心方法：供 BirdTrigger 在到达目的地后调用。
    /// </summary>
    public void OnBirdArrived(GameObject bird)
    {
        if (_hasCompleted)
        {
            // 如果已经完成，忽略后续的到达消息
            return;
        }

        _arrivedBirdCount++;
        Debug.Log($"小鸟 {bird.name} 已到达。当前进度: {_arrivedBirdCount}/{totalBirds}");

        // 检查是否所有小鸟都已到达
        if (_arrivedBirdCount >= totalBirds)
        {
            CompleteSequence();
        }
    }

    /// <summary>
    /// 所有小鸟都到达后，执行最终序列。
    /// </summary>
    // private void CompleteSequence()
    // {
    //     _hasCompleted = true;
    //     Debug.Log("所有小鸟已归巢！");

    //     // 1. 激活目标物体（如桥）
    //     if (objectToActivateOnComplete != null)
    //     {
    //         objectToActivateOnComplete.SetActive(true);
    //     }

    //     // 2. 解锁玩家（如果需要）
    //     if (unlockPlayerOnComplete)
    //     {
    //         UnlockPlayerMovement();
    //     }

    //     // 3. 【可扩展】在这里添加其他逻辑
    //     //    - 播放胜利音效
    //     //    - 触发过场动画
    //     //    - 更新游戏状态等
    // }
    private void CompleteSequence()
    {
        _hasCompleted = true;
        Debug.Log("所有小鸟已归巢！");

        // 1. 激活目标物体（如桥）
        if (objectToActivateOnComplete != null)
        {
            objectToActivateOnComplete.SetActive(true);
        }

        // 2. 解锁玩家（如果需要）
        if (unlockPlayerOnComplete)
        {
            UnlockPlayerMovement();
        }

        // 👇 新增：禁用整个鸟群（包括四只小鸟）
        gameObject.SetActive(false); // 因为 BirdCoordinator 挂在 BirdsFlock 上，所以禁用自身即可
    }

    /// <summary>
    /// 解锁玩家的移动控制。
    /// </summary>
    private void UnlockPlayerMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            // 假设你的玩家脚本名为 PlayerMovement
            PlayerController playerScript = player.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                // 调用玩家脚本中的解锁方法
                // 你需要确保 PlayerMovement 脚本中有类似 LockMovement(bool) 的公共方法
                playerScript.canMove = true; // false 表示解锁
            }
            else
            {
                Debug.LogWarning("未找到 PlayerMovement 脚本！");
            }
        }
        else
        {
            Debug.LogWarning($"未找到 Tag 为 '{playerTag}' 的玩家对象！");
        }
    }
}