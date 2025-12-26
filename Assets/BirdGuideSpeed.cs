using UnityEngine;

public class BirdGuideSpeed : MonoBehaviour
{
    private Animator animator;

    [Header("控制参数")]
    [Range(0, 5f)] // 在编辑器里生成一个 0 到 5 倍速的滑动条
    public float playSpeed = 1.0f;

    void Start()
    {
        // 获取物体上的 Animator 组件
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 实时更新播放速度
        animator.speed = playSpeed;

        // 快捷键测试示例
        if (Input.GetKeyDown(KeyCode.Space)) // 按空格暂停/恢复
        {
            playSpeed = (playSpeed == 0) ? 1.0f : 0;
        }
    }
}