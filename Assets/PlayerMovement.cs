using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("特效设置")]
    public GameObject dashTrailObject; 

    [Header("起跳设置")]
    public float jumpDelay = 0.15f; 
    private bool isJumping;        

    [Header("功能开关")]
    public bool canMove = true;
    public bool canJump = true;
    public bool canDoubleJump = true;
    public bool canDash = true;
    public bool canSing = true; 

    [Header("物理参数")]
    public float walkSpeed = 8f;
    public float dashSpeed = 14f;
    public float jumpForce = 12f;

    [Header("冲刺冷却设置")]
    public float dashDuration = 1f;    // 冲刺持续时间
    public float dashCooldown = 1f;    // 冲刺冷却时间
    private float dashTimer;           // 冲刺计时器
    private float cooldownTimer;       // 冷却计时器
    private bool isDashCooldown;       // 是否处于冷却中

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
    public bool isGrounded;
    private float horizontalInput;
    private bool isDashing; 
    private bool isSinging; 

    [Header("动画平滑设置")]
    public float hangTimeThreshold = 0.5f; 

    // 1. 在 Header 增加一个变量
    [Header("视觉修正")]
    public float trailExtraTime = 0.1f; // 拖尾多留存的时间
    private float trailTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround(); // 必须首先更新地面状态
        HandleDashTimers();

        // 1. 修改：不仅要检测 isGrounded，还要确保垂直速度接近0（防止跳跃瞬间判定还在地面）
        bool strictlyOnGround = isGrounded && Mathf.Abs(rb.velocity.y) < 0.1f;

        // 2. 强制拦截：如果不在地面，立刻强制结束唱歌状态
        if (!strictlyOnGround)
        {
            isSinging = false;
        }

        // 3. 处理唱歌输入（只有在严格的地面判定下才有效）
        HandleSingInput(strictlyOnGround); 

        if (!isSinging)
        {
            if (canMove) HandleMovementInput();
            if (canJump) HandleJumpInput();
        }
        else 
        {
            // 唱歌时强制清空输入，防止带惯性滑行
            horizontalInput = 0;
            if (isDashing) StopDash(); 
        }
        
        UpdateAnimation(); 
        Flip();
    }

    void HandleSingInput(bool strictlyOnGround)
    {
        // 如果功能关闭或不在地面，直接跳出
        if (!canSing || !strictlyOnGround) 
        {
            isSinging = false;
            return;
        }

        if (Input.GetKey(KeyCode.O))
        {
            isSinging = true;
        }
        else
        {
            isSinging = false;
        }
    }
    void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 增加 !isSinging 判定，确保唱歌时按 Shift 无效
        if (canDash && !isDashCooldown && !isDashing && !isSinging &&
           (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && 
           horizontalInput != 0)
        {
            StartDash();
        }

        if (isDashing && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            StopDash();
        }
    }

    void FixedUpdate()
    {
        if (canMove && !isSinging) 
        {
            ApplyMovement();
        }
        else 
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && rb.velocity.y <= 0.1f)
        {
            jumpsLeft = canDoubleJump ? 2 : 1;
        }
    }

    // 2. 修改 HandleDashTimers 逻辑
    void HandleDashTimers()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            trailTimer = trailExtraTime; // 冲刺时不断重置视觉计时器
            if (dashTimer <= 0)
            {
                StopDash();
            }
        }
        else
        {
            // 冲刺停止后，开始消耗拖尾多留存的时间
            if (trailTimer > 0) trailTimer -= Time.deltaTime;
        }

        if (isDashCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0) isDashCooldown = false;
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
    }

    void StopDash()
    {
        isDashing = false;
        isDashCooldown = true;
        cooldownTimer = dashCooldown;
    }


    void ApplyMovement()
    {
        float targetSpeed = isDashing ? dashSpeed : walkSpeed;
        rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
    }

    // ... (HandleJumpInput, JumpRoutine, PerformJumpPhysics, Flip, UpdateAnimation 保持不变)
    
    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && !isJumping && !isSinging)
        {
            if (isGrounded || jumpsLeft > 0)
            {
                StartCoroutine(JumpRoutine());
            }
        }
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;

        if (isGrounded)
        {
            anim.SetTrigger("jump"); 
            canMove = false;
            yield return new WaitForSeconds(jumpDelay); 
            PerformJumpPhysics(); 
            canMove = true;
        }
        else
        {
            anim.SetTrigger("doubleJump");
            rb.velocity = new Vector2(rb.velocity.x, 0);
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0; 

            yield return new WaitForSeconds(0.3f); 

            rb.gravityScale = originalGravity; 
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
        if (isSinging) return; 
        if (horizontalInput > 0) transform.localScale = new Vector3(20, 20, 1);
        else if (horizontalInput < 0) transform.localScale = new Vector3(-20, 20, 1);
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        float moveSpeed = isGrounded ? Mathf.Abs(horizontalInput) : 0;
        anim.SetFloat("speed", moveSpeed);

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isDashing", isDashing); 
        anim.SetBool("isSinging", isSinging); 

        bool isHanging = !isGrounded && rb.velocity.y > 0 && rb.velocity.y < hangTimeThreshold;
        anim.SetBool("isHanging", isHanging);
        
        // 修改：只要还在冲刺，或者冲刺刚结束的 0.1s 内，都显示拖尾
        if (dashTrailObject != null)
        {
            bool showTrail = isDashing || trailTimer > 0;
            dashTrailObject.SetActive(showTrail);
        }
    }
}