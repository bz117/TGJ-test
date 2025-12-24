using UnityEngine;

public class TriggerMove : MonoBehaviour
{
    public float speed = 4;
    public float height = 600;
    
    // 控制开关：默认为 false，触发后变为 true
    private bool isMoving = false; 

    private Rigidbody2D rb;
    private float startY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        startY = transform.position.y;
    }

    void FixedUpdate()
    {
        // 如果还没有被触发，则跳过移动逻辑
        if (!isMoving) 
        {
            rb.velocity = Vector2.zero; // 确保静止
            return;
        }

        float targetY = startY + Mathf.Sin(Time.time * speed) * height;
        float nextV = (targetY - rb.position.y) / Time.fixedDeltaTime;
        rb.velocity = new Vector2(rb.velocity.x, nextV);
    }

    // 当有物体进入触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
        isMoving = true;
        }
        
        // 如果你希望每次触发时都从起始位置重新开始计算波形，
        // 可以在这里重置 Time.time 的偏移量，或者记录当前时间。
    }
}