using System.Collections; // 必须引用，用于协程
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("起跳设置")]
    public float jumpDelay = 0.15f; // 下蹲动作持续的时间（秒）
    private bool isJumping;        // 标记是否正在跳跃过程中

    [Header("功能开关")]
    public bool canMove = true;
    public bool canJump = true;
    public bool canDoubleJump = true;
    public bool canDash = true;

    [Header("物理参数")]
    public float walkSpeed = 8f;
    public float dashSpeed = 14f;
    public float jumpForce = 12f;

    [Header("检测设置")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("特效设置")]
    public GameObject jumpEffect;    
    public Transform effectPoint;    
    private Rigidbody2D rb;
    private Animator anim;
    private int jumpsLeft;
    private bool isGrounded;
    private float horizontalInput;

    [Header("动画平滑设置")]
    public float hangTimeThreshold = 0.5f; // 垂直速度在这个范围内时，视为滞空状态

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        if (canMove) HandleMovementInput();
        if (canJump) HandleJumpInput();

        UpdateAnimation(); 
        Flip();
    }

    void FixedUpdate()
    {
        if (canMove) ApplyMovement();
        else rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && rb.velocity.y <= 0.1f)
        {
            jumpsLeft = canDoubleJump ? 2 : 1;
        }
    }

    void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void ApplyMovement()
    {
        bool isDashing = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float targetSpeed = (canDash && isDashing) ? dashSpeed : walkSpeed;
        rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            if (isGrounded || jumpsLeft > 0)
            {
                // 开启跳跃协程，不再直接执行 Jump()
                StartCoroutine(JumpRoutine());
            }
        }
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;

        if (isGrounded)
        {
            // --- 地面起跳逻辑 ---
            anim.SetTrigger("jump"); // 播放带有下蹲动作的起跳动画
            
            // 关键：在下蹲动作结束前，角色不移动
            // 如果你想彻底锁定水平移动，可以在此处临时设置 canMove = false;
            canMove = false;
            yield return new WaitForSeconds(jumpDelay); // 等待下蹲动画结束
            
            PerformJumpPhysics(); // 真正施加物理力
            canMove = true;
        }
        else
        {
            // --- 二段跳逻辑（通常不需要延迟下蹲） ---
            anim.SetTrigger("doubleJump");
            rb.velocity = new Vector2(rb.velocity.x, 0); // 清空垂直速度确保手感一致
            PerformJumpPhysics();
        }

        isJumping = false;
    }

    void PerformJumpPhysics()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpsLeft--;
        
        if (!isGrounded && jumpEffect != null)
        {
            Instantiate(jumpEffect, effectPoint.position, Quaternion.identity);
        }
    }

    void Flip()
    {
        if (horizontalInput > 0) transform.localScale = new Vector3(20, 20, 1);
        else if (horizontalInput < 0) transform.localScale = new Vector3(-20, 20, 1);
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);

        // --- 修改后的逻辑：仅在向上移动且速度较慢时播放滞空动画 ---
        // 条件 1: !isGrounded (在空中)
        // 条件 2: rb.velocity.y > 0 (正在向上移动)
        // 条件 3: rb.velocity.y < hangTimeThreshold (速度减慢，接近最高点)
        bool isHanging = !isGrounded && rb.velocity.y > 0 && rb.velocity.y < hangTimeThreshold;
        
        anim.SetBool("isHanging", isHanging);
    }
}