using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        // 获取 Rigidbody2D 组件
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. 获取输入（与 3D 类似）
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D
        float moveY = Input.GetAxisRaw("Vertical");   // W/S

        // 2. 计算速度向量
        Vector2 moveVector = new Vector2(moveX, moveY).normalized;
        
        // 3. 使用 Rigidbody2D 移动（推荐用于物理交互）
        // 这种方法通常放在 FixedUpdate() 中以获得更稳定的物理效果
        rb.velocity = moveVector * moveSpeed;
    }
}