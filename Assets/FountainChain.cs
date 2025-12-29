using UnityEngine;

public class FountainChain : MonoBehaviour
{
    [Header("移动参数")]
    public float speed = 3f;
    public float height = 5f; 
    public bool isActive = false; // 第一个喷泉在 Inspector 里手动勾选此项

    [Header("计时与联动")]
    public FountainChain nextFountain; 
    public float targetTime = 2.0f;    
    public float tolerance = 0.5f;     
    
    private Rigidbody2D rb;
    private float startY;
    private float timer = 0f;
    private bool isTimerRunning = false;
    private bool isFailed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        startY = transform.position.y;
        
        // 如果勾选了 isActive，初始速度设为 0 而不是静止
        if (!isActive) rb.velocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            float targetY = startY + Mathf.Sin(Time.time * speed) * height;
            float nextV = (targetY - rb.position.y) / Time.fixedDeltaTime;
            rb.velocity = new Vector2(rb.velocity.x, nextV);
        }

        if (isTimerRunning && !isFailed)
        {
            timer += Time.fixedDeltaTime;
            if (timer > (targetTime + tolerance))
            {
                Debug.Log("<color=red>【超时失败】</color> 必须返回起点喷泉重新触发！");
                isFailed = true;
                isTimerRunning = false;
            }
        }
    }

    // 玩家踩到喷泉（作为起点时触发）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            Debug.Log("<color=cyan>【计时开始】</color> 正在前往下一个目标...");
            isFailed = false;
            isTimerRunning = true;
            timer = 0f;
        }
    }

    public void CheckSuccess()
    {
        if (isFailed)
        {
            Debug.Log("<color=red>【拦截】</color> 挑战已失效，请先回到喷泉重置计时。");
            return;
        }
        if (!isTimerRunning) return;

        float minTime = targetTime - tolerance;
        float maxTime = targetTime + tolerance;

        if (timer >= minTime && timer <= maxTime)
        {
            Debug.Log("<color=green>【挑战成功！】</color>");
            if (nextFountain != null) nextFountain.isActive = true;
            isTimerRunning = false;
        }
        else if (timer < minTime)
        {
            Debug.Log("<color=orange>【太快失败！】</color> 请回到起点重新开始。");
            isFailed = true;
            isTimerRunning = false;
        }
    }
}