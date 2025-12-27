using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    private bool isDashing; 
    private bool isSinging; 

    [Header("动画平滑设置")]
    public float hangTimeThreshold = 0.5f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        
        // 只有不在唱歌时，才允许处理移动和跳跃输入
        if (!isSinging)
        {
            if (canMove) HandleMovementInput();
            if (canJump) HandleJumpInput();
        }
        
        HandleSingInput(); 
        UpdateAnimation(); 
        Flip();
    }

    void FixedUpdate()
    {
        // 唱歌时物理速度归零（y轴保留重力影响）
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

    void HandleMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        // 冲刺判定：按住 Shift 且有左右输入
        isDashing = canDash && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && horizontalInput != 0;
    }

    void HandleSingInput()
    {
        if (!canSing || !isGrounded) 
        {
            isSinging = false;
            return;
        }

        // 修改此处：现在使用 O 键唱歌
        if (Input.GetKey(KeyCode.O))
        {
            isSinging = true;
            horizontalInput = 0; 
        }
        else
        {
            isSinging = false;
        }
    }

    void ApplyMovement()
    {
        float targetSpeed = isDashing ? dashSpeed : walkSpeed;
        rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
    }

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

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        
        // 传递新增的动画布尔参数
        anim.SetBool("isDashing", isDashing); 
        anim.SetBool("isSinging", isSinging); 

        bool isHanging = !isGrounded && rb.velocity.y > 0 && rb.velocity.y < hangTimeThreshold;
        anim.SetBool("isHanging", isHanging);
    }
}