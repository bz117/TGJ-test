using UnityEngine;

/// <summary>
/// 挂在每只小鸟上的触发器脚本。
/// 负责：1. 检测声波；2. 激活自身；3. 移动到目标点；4. 到达后通知协调者。
/// </summary>
public class BirdTrigger : MonoBehaviour
{
    [Header("激活延迟")]
    public float delayAfterActivation = 0.5f; // 在 Inspector 中可调整的延迟时间（秒）
    private float _activationTimer = 0f;
    private bool _isDelaying = false; // 新增状态：是否处于延迟中
    [Header("移动设置")]
    public Transform destination;      // 所有小鸟共同的目标点
    public float moveSpeed = 5f;
    public Transform birdsFlock;       // 鸟群父物体（可选，用于归位）

    [Header("通信设置")]
    public BirdCoordinator coordinator; // 拖入挂有 BirdCoordinator 的 GameObject（通常是 birdsFlock）

    [Header("视觉/动画")]
    public Animator animator;
    public string animationTriggerName = "BirdActivate";
    [Range(0, 1)] public float targetAlpha = 1f;

    private SpriteRenderer _spriteRenderer;
    private bool _isActivated = false;
    private bool _hasArrived = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetSpriteAlpha(0.4f); // 初始隐藏
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActivated && other.CompareTag("SoundWave"))
        {
            Activate();
        }
    }

    private void Activate()
    {
        _isActivated = true;
        _isDelaying = true; // 👈 开始延迟
        _activationTimer = 0f; // 重置计时器

        SetSpriteAlpha(targetAlpha);
        PlayAnimation();

        // 可选：如果小鸟有 Rigidbody2D，可以在此冻结或设置为 Kinematic
        // Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // if (rb != null) rb.bodyType = RigidbodyBodyType.Kinematic;
    }
    void Update()
    {
        if (!_isActivated) return;

        // 🕒 状态1：正在延迟
        if (_isDelaying)
        {
            _activationTimer += Time.deltaTime;
            if (_activationTimer >= delayAfterActivation)
            {
                _isDelaying = false; // 延迟结束
                // 可选：播放起飞音效或第二段动画
            }
        }
        // 🚀 状态2：延迟结束，开始移动
        else if (!_hasArrived && destination != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination.position) < 0.01f)
            {
                _hasArrived = true;
                OnArrival();
            }
        }
    }
    private void OnArrival()
    {
        // 归位到鸟群（可选）
        if (birdsFlock != null)
        {
            transform.SetParent(birdsFlock);
        }

        // 👇 核心：通知协调者“我到了！”
        if (coordinator != null)
        {
            coordinator.OnBirdArrived(gameObject);
        }
        else
        {
            Debug.LogError($"[BirdTrigger] {name} 的 Coordinator 未指定！");
        }
    }

    void PlayAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            animator.SetTrigger(animationTriggerName);
        }
    }

    void SetSpriteAlpha(float alpha)
    {
        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a = alpha;
            _spriteRenderer.color = c;
        }
    }
}