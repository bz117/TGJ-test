using UnityEngine;

public class CloudMove_x : MonoBehaviour
{
    public float speed = 1;
    public float width = 300;
    
    private Rigidbody2D rb;
    private float startX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 记得把刚体的 Gravity Scale 设为 0，否则物体会受重力掉下去
        rb.gravityScale = 0;
        startX = transform.position.y;
    }

    void FixedUpdate() // 物理移动建议写在 FixedUpdate 中
    {
        // 计算目标高度
        float targetX = startX + Mathf.Sin(Time.time * speed) * width;
        
        // 计算到达目标点所需的速度
        float nextVX = (targetX - rb.position.y) / Time.fixedDeltaTime;
        
        // 只改变垂直速度，保留当前的水平速度
        rb.velocity = new Vector2(nextVX, rb.velocity.y);
    }
}