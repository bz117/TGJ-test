using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("特效设置")]
    public GameObject dashTrailObject; 

    [Header("起跳设置")]
    private bool hasPerformedFirstJump = false;
    public float jumpDelay = 0.0f; 
    private bool isJumping;        

    [Header("功能开关")]
    public bool canMove = true;
    public bool canJump = true;
    public bool canDoubleJump = true;
    public bool canDash = true;
    public bool canSing = true; 

    [Header("物理参数")]
    public float walkSpeed = 5f; // 修正：合理行走速度
    public float dashSpeed = 12f; // 修正：合理冲刺速度
    public float jumpForce = 8f; // 修正：合理跳跃力度

    [Header("冲刺冷却设置")]
    public float dashDuration = 1f;    // 冲刺持续时间
    public float dashCooldown = 1f;    // 冲刺冷却时间
    private float dashTimer;           // 冲刺计时器
    private float cooldownTimer;       // 冷却计时器
    private bool isDashCooldown;       // 是否处于冷却中

    [Header("检测设置")]
    public Transform groundCheck;
    public float checkRadius = 10;
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
    public float hangTimeThreshold = 0.24f; 

    [Header("视觉修正")]
    public float trailExtraTime = 0.1f; // 拖尾多留存的时间
    private float trailTimer;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.15f;
    public float groundCheckHorizontalOffset = 0.2f; // 左右偏移量

    void CheckGround()
    {
        if (groundCheck == null)
        {
            Debug.LogWarning("groundCheck 未赋值！", this);
            isGrounded = false;
            return;
        }

        Vector2 originCenter = groundCheck.position;
        Vector2 originLeft = originCenter + Vector2.left * groundCheckHorizontalOffset;
        Vector2 originRight = originCenter + Vector2.right * groundCheckHorizontalOffset;
        Vector2 direction = Vector2.down;
        float distance = groundCheckDistance;

        bool hitLeft = Physics2D.Raycast(originLeft, direction, distance, groundLayer);
        bool hitCenter = Physics2D.Raycast(originCenter, direction, distance, groundLayer);
        bool hitRight = Physics2D.Raycast(originRight, direction, distance, groundLayer);

        // 调试可视化
        Debug.DrawRay(originLeft, direction * distance, hitLeft ? Color.green : Color.red);
        Debug.DrawRay(originCenter, direction * distance, hitCenter ? Color.green : Color.red);
        Debug.DrawRay(originRight, direction * distance, hitRight ? Color.green : Color.red);

        isGrounded = hitLeft || hitCenter || hitRight;

        bool strictlyOnGround = isGrounded && Mathf.Abs(rb.velocity.y) < 0.1f;
        if (strictlyOnGround)
        {
            jumpsLeft = canDoubleJump ? 2 : 1;
            hasPerformedFirstJump = false;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 验证组件是否获取成功
        if (rb == null)
        {
            Debug.LogError("角色缺少Rigidbody2D组件！", this);
        }
        if (anim == null)
        {
            Debug.LogWarning("角色缺少Animator组件！", this);
        }

        // 初始化跳跃次数
        jumpsLeft = canDoubleJump ? 2 : 1;
    }

    void Update()
    {
        CheckGround(); // 必须首先更新地面状态
        HandleDashTimers();

        // 1. 修改：不仅要检测 isGrounded，还要确保垂直速度接近0
        bool strictlyOnGround = isGrounded && Mathf.Abs(rb.velocity.y) < 0.1f;

        // 2. 强制拦截：如果不在地面，立刻强制结束唱歌状态
        if (!strictlyOnGround)
        {
            isSinging = false;
        }

        // 3. 处理唱歌输入（只有在严格的地面判定下才有效）
        HandleSingInput(strictlyOnGround); 

        // 监控关键状态，方便排查问题
        //Debug.Log($"isSinging: {isSinging}, canMove: {canMove}, horizontalInput: {horizontalInput}", this);

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
        // 如果功能关闭或不在地面，强制重置唱歌状态
        if (!canSing || !strictlyOnGround) 
        {
            isSinging = false;
            return;
        }

        // 仅当主动按下O键时开启，松开立即关闭
        isSinging = Input.GetKey(KeyCode.O);
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
        if (canMove && !isSinging && rb != null) 
        {
            ApplyMovement();
        }
        else if (rb != null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

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
        dashTrailObject?.SetActive(true);
        isDashing = true;
        dashTimer = dashDuration;
    }

    void StopDash()
    {
        dashTrailObject?.SetActive(false);
        isDashing = false;
        isDashCooldown = true;
        cooldownTimer = dashCooldown;
    }

    void ApplyMovement()
    {
        float targetSpeed = isDashing ? dashSpeed : walkSpeed;
        // 赋值水平速度，保留垂直速度
        rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && !isJumping && !isSinging)
        {
            // 一段跳：必须严格在地面且尚未跳过
            if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.1f && !hasPerformedFirstJump)
            {
                StartCoroutine(JumpRoutine(firstJump: true));
            }
            // 二段跳：必须已跳过一次，且还有剩余跳次数，且不在地面
            else if (canDoubleJump && !isGrounded && hasPerformedFirstJump && jumpsLeft > 0)
            {
                StartCoroutine(JumpRoutine(firstJump: false));
            }
        }
    }

    IEnumerator JumpRoutine(bool firstJump)
    {
        isJumping = true;

        if (firstJump)
        {
            anim?.SetTrigger("jump");
            canMove = false;
            yield return new WaitForSeconds(jumpDelay);
            PerformJumpPhysics();
            canMove = true;
            hasPerformedFirstJump = true; // 标记已执行一段跳
            jumpsLeft--; // 消耗一次跳跃机会（从2→1）
        }
        else
        {
            anim?.SetTrigger("doubleJump");
            rb.velocity = new Vector2(rb.velocity.x, 0);
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0;
            yield return new WaitForSeconds(0.3f);
            rb.gravityScale = originalGravity;
            PerformJumpPhysics();
            jumpsLeft--; // 二段跳消耗最后一次（1→0）
        }

        isJumping = false;
    }

    void PerformJumpPhysics()
    {
        if (rb == null) return;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (!isGrounded && jumpEffect != null && effectPoint != null)
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
        anim.SetFloat("yVelocity", rb != null ? rb.velocity.y : 0);
        anim.SetBool("isDashing", isDashing); 
        anim.SetBool("isSinging", isSinging); 

        bool isHanging = !isGrounded && (rb != null ? rb.velocity.y > 0 && rb.velocity.y < hangTimeThreshold : false);
        anim.SetBool("isHanging", isHanging);
        
        // 修改：只要还在冲刺，或者冲刺刚结束的 0.1s 内，都显示拖尾
        if (dashTrailObject != null)
        {
            bool showTrail = isDashing || trailTimer > 0;
            dashTrailObject.SetActive(showTrail);
        }
    }
}