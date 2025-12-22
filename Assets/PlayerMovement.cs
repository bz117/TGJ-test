using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("二段跳设置")]
    public int maxJumps = 2; // 最大跳跃次数
    private int jumpsLeft;   // 剩余跳跃次数

    [Header("地面检测")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        // 1. 地面检测：判断是否踩在地面上
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpsLeft = maxJumps; // 回到地面重置次数
        }

        // 2. 左右移动逻辑 (保留原来的左右，移除上下对速度的控制)
        float moveX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // 3. 跳跃逻辑 (检测 W 键或空格)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void Jump()
    {
        if (jumpsLeft > 0)
        {
            // 清除之前的垂直速度，确保二段跳力道一致
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            jumpsLeft--;
        }
    }

    // 在编辑器里画出检测范围，方便调试
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}