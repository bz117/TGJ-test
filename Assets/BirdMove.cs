using UnityEngine;
using System.Collections.Generic;

public class BirdMove : MonoBehaviour
{
    // 用来存放已经触发状态的发送者
    private HashSet<BirdTrigger> activeSenders = new HashSet<BirdTrigger>();
    public int targetCount = 4; // 目标数量
    public bool isTouched = false;
    private int isBack = 0;
    public Transform destination; // 在 Inspector 中拖入这个物体要去的“终点”
    public float moveSpeed = 5f;  // 移动速度

    public void RegisterSender(BirdTrigger sender)
    {
        // 如果这个发送者还没记录过，就加进去
        if (!activeSenders.Contains(sender))
        {
            activeSenders.Add(sender);
            Debug.Log($"已收集: {activeSenders.Count} / {targetCount}");

            // 检查是否达到 4 个
            if (activeSenders.Count >= targetCount)
            {
                NotifyAllSenders();
            }
        }
    }

    void NotifyAllSenders()
    {
        Debug.Log("全部就绪，正在发送回执消息...");
        
        // 遍历所有记录下的发送者，调用它们的方法
        foreach (BirdTrigger s in activeSenders)
        {
            s.OnFinalAction();
        }

        // 如果需要重新开始，可以清空列表
        // activeSenders.Clear();
    }

    // 提供一个方法让别的脚本调用
    public void UpdateStatus(string objectName, bool status)
    {
        isTouched = status;
        Debug.Log(objectName + " 发来了状态：" + status);
    }
    // 这个方法会在子物体执行 SendMessage 时被触发
    public void OnChildArrived(GameObject child)
    {
        Debug.Log($"父物体 {gameObject.name} 收到信号：子物体 {child.name} 已挂载。");
        
        // 示例：每当有一个子物体归位，就把父物体变大一点
        isBack += 1;
    }
    public void Update()
    {
        if(isBack == 4)
        {
            // 平滑移动向目标点
            transform.position = Vector3.MoveTowards(
                transform.position, 
                destination.position, 
                moveSpeed * Time.deltaTime
            );
        }
    }
}