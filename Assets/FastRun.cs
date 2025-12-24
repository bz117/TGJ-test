using UnityEngine;

public class FastRun : MonoBehaviour
{
    [Header("基础移动")]
    public float walkSpeed = 8f;
    public float dashSpeed = 14f; // 冲刺时的速度
    private float currentSpeed;

    // [Header("跳跃设置")]
    // public float jumpForce = 12f;
    // private int jumpsLeft;
    // public int maxJumps = 2;

    [Header("地面检测")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // 1. 冲刺输入检测
        HandleDashInput();

        // 2. 地面检测与跳跃次数重置
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        // if (isGrounded && rb.velocity.y <= 0)
        // {
        //     jumpsLeft = maxJumps;
        // }

        // 3. 跳跃处理
        // if (Input.GetButtonDown("Jump") && (isGrounded || jumpsLeft > 0))
        // {
        //     rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        //     jumpsLeft--;
        // }

        // 4. 动画参数更新
        //UpdateAnimations();
    }

    void FixedUpdate()
    {
        // 5. 执行移动
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        // 翻转角色
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(moveInput * 20, 20, 0);
        }
    }

    void HandleDashInput()
    {
        // 按住 LeftShift 时使用冲刺速度，否则恢复行走速度
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = dashSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    // void UpdateAnimations()
    // {
    //     if (anim != null)
    //     {
    //         float horizontalInput = Input.GetAxisRaw("Horizontal");
    //         // 将当前实际速度绝对值传给动画机
    //         // 如果你希望冲刺有专门的动画，可以再增加一个 isDashing 的 Bool 参数
    //         anim.SetFloat("speed", Mathf.Abs(horizontalInput * currentSpeed));
    //         anim.SetBool("isGrounded", isGrounded);
            
    //         // 如果你设置了冲刺专用动画，可以取消下面注释
    //         // anim.SetBool("isDashing", Input.GetKey(KeyCode.LeftShift) && horizontalInput != 0);
    //     }
    // }
}