using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("功能开关")]
    public bool canMove = true;      // 是否能移动
    public bool canJump = true;      // 是否能跳跃
    public bool canDoubleJump = true;// 是否允许二段跳
    public bool canDash = true;      // 是否能冲刺加速

    [Header("物理参数")]
    public float walkSpeed = 8f;
    public float dashSpeed = 14f;
    public float jumpForce = 12f;

    [Header("检测设置")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    // 内部状态
    private Rigidbody2D rb;
    private Animator anim;
    private int jumpsLeft;
    private bool isGrounded;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 始终更新地面检测
        CheckGround();

        // 处理功能模块
        if (canMove) HandleMovementInput();
        if (canJump) HandleJumpInput();

        //UpdateAnimation();
        Flip();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            ApplyMovement();
        }
        else
        {
            // 如果禁用移动，则水平速度归零，但保留垂直速度（如重力）
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // --- 功能逻辑拆分 ---

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && rb.velocity.y <= 0.1f)
        {
            // 如果没开启二段跳，最大次数就是1，否则是2
            jumpsLeft = canDoubleJump ? 2 : 1;
        }
    }

    void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void ApplyMovement()
    {
        // 判断冲刺逻辑：只有开启了 canDash 且按住 Shift 才会变快
        bool isDashing = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetSpeed = (canDash && isDashing) ? dashSpeed : walkSpeed;
        rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            // 如果在地板上，或者还有剩余跳跃次数
            if (isGrounded || jumpsLeft > 0)
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpsLeft--;
    }

    // --- 辅助功能 ---

    void Flip()
    {
        if (horizontalInput > 0) transform.localScale = new Vector3(20, 20, 0);
        else if (horizontalInput < 0) transform.localScale = new Vector3(-20, 20, 0);
    }

    // void UpdateAnimation()
    // {
    //     if (anim == null) return;
    //     anim.SetFloat("speed", Mathf.Abs(horizontalInput));
    //     anim.SetBool("isGrounded", isGrounded);
    //     anim.SetFloat("verticalVelocity", rb.velocity.y);
    // }
}