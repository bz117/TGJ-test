using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [Header("移动设置")]
    public Vector2 moveDistance = new Vector2(5f, 0f); // 移动的向量（默认水平移动 5 个单位）
    public float speed = 2f; // 移动速度

    public float max_x;
    public float max_y;
    public float min_x;
    public float min_y;


    private Vector2 startPosition;

    void Start()
    {
        // 记录初始位置
        startPosition = transform.position;
    }

    void Update()
    {
        // 计算往复值 (0 到 1 之间循环)
        float movementFactor = Mathf.PingPong(Time.time * speed, 1f);

        // 根据初始位置和偏移量计算新位置
        Vector2 targetPosition = startPosition + (moveDistance * movementFactor);

        // 应用位置
        transform.position = targetPosition;
    }

}