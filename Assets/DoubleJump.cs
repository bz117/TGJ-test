using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 公开变量，可以在 Unity Inspector 中调整
    public float jumpForce = 10f; // 每次跳跃的力量
    public int maxJumps = 2; // 最大跳跃次数 (2 表示二段跳)

    // 私有变量
    private Rigidbody2D rb;
    private int remainingJumps;
    private bool isGrounded; // 辅助判断是否在地面上

    // 用于地面检测的设置
    public Transform groundCheck; // 用于检测地面的空物体的位置
    public float checkRadius = 0.2f; // 检测半径
    public LayerMask whatIsGround; // 定义哪些层是地面

    void Start()
    {
        // 获取 Rigidbody2D 组件
        rb = GetComponent<Rigidbody2D>();
        // 初始化剩余跳跃次数
        remainingJumps = maxJumps;
    }

    void Update()
    {
        // 1. 地面检测
        // 使用 Physics2D.OverlapCircle 检测 groundCheck 附近是否有地面层的 Collider
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 如果在地面上，重置跳跃次数
        if (isGrounded)
        {
            remainingJumps = maxJumps;
        }

        // 2. 跳跃输入和逻辑
        // 检查是否按下跳跃键 (默认是 Space)
        if (Input.GetButtonDown("Jump"))
        {
            // 只有当剩余跳跃次数大于 0 时才允许跳跃
            if (remainingJumps > 0)
            {
                // 清除当前的垂直速度，确保每次跳跃都获得一个完整的推力
                rb.velocity = new Vector2(rb.velocity.x, 0f); 
                
                // 施加向上的跳跃力
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                // 减少剩余跳跃次数
                remainingJumps--;

                // 可以在这里添加跳跃音效或特效
                // Debug.Log("Jump executed. Remaining jumps: " + remainingJumps);
            }
        }
    }
}