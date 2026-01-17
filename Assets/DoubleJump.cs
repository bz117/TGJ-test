using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("检测设置")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim; // 引用动画组件
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // 初始化
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // if (isGrounded && rb.velocity.y <= 0)
        // {
        //     jumpsLeft = maxJumps;
        // }

        // // 跳跃输入
        // if (Input.GetButtonDown("Jump") && (isGrounded || jumpsLeft > 0))
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        //     jumpsLeft--;
        //     // 触发跳跃动画触发器（如果有的话）
        //     // anim.SetTrigger("takeOff"); 
        // }

        // --- 动画处理核心逻辑 ---
        
        // // 1. 跑步动画：Mathf.Abs 获取绝对值，左右走动速度都大于 0.1 时切换
        // anim.SetFloat("speed", Mathf.Abs(horizontalInput));

        // // 2. 地面状态：告诉动画机我们是否在地上
        // anim.SetBool("isGrounded", isGrounded);

        // // 3. 垂直速度：用于在空中切换“上升”和“下落”动画
        // anim.SetFloat("verticalVelocity", rb.velocity.y);

        // 角色转向
        Flip();
    }

    void FixedUpdate()
    {
        if(isGrounded)
        {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    void Flip()
    {
        if (horizontalInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
}