using UnityEngine;

public class BirdTrigger : MonoBehaviour
{
    public Transform birds_flock;
    private bool isTriggered = false;
    private SpriteRenderer _spriteRenderer;

    public BirdMove receiver; // 在 Inspector 中拖入接收者物体
    public Transform destination; // 在 Inspector 中拖入这个物体要去的“终点”
    public float moveSpeed = 5f;  // 移动速度
    
    private bool shouldMove = false; // 是否开始移动的开关

    // 当发生碰撞时触发（物体必须有 Collider，且至少一个有 Rigidbody）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTriggered == false && other.CompareTag("SoundWave"))
        {
            // 检查碰撞对象的标签（可选，避免任何东西碰到都变色）
            SetSpriteAlpha(1.0f); 
            // 2. 将“自己”传给接收者
            if (receiver != null) {
                receiver.RegisterSender(this); 
            }
            isTriggered = true;
        }
    }

    // 被接收者调用的回执方法
    public void OnFinalAction()
    {
        shouldMove = true; // 开启移动开关
        Debug.Log(gameObject.name + " 开始向目标移动！");
    }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void SetSpriteAlpha(float alpha)
    {
        // 不能直接修改 _spriteRenderer.color.a，必须先取出来赋值给临时变量
        Color tempColor = _spriteRenderer.color;
        tempColor.a = alpha;
        _spriteRenderer.color = tempColor;
    }

    void Update() {
        // 如果开关打开，且目标点不为空
        if (shouldMove && destination != null) {
            // 平滑移动向目标点
            transform.position = Vector3.MoveTowards(
                transform.position, 
                destination.position, 
                moveSpeed * Time.deltaTime
            );

            // 如果到达目标点，关闭开关（可选）
            if (Vector3.Distance(transform.position, destination.position) < 0.01f) 
            {
                shouldMove = false;
                transform.SetParent(birds_flock);

                // 2. 核心操作：重新获得碰撞体积
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) {
                rb.simulated = true;         // 重新开启物理模拟
                rb.bodyType = RigidbodyType2D.Kinematic; // 设为运动学，防止受重力掉落，但仍能触发碰撞检测
                rb.velocity = Vector2.zero; // 速度清零
            }

            Collider2D col = GetComponent<Collider2D>();
            if (col != null) {
                col.enabled = true; // 确保碰撞器是开启的
                col.isTrigger = false; // 如果你想让它挡住别人，确保不是 Trigger
            }
            // 2. 向父物体发送消息
            // transform.parent 就是你刚才设置的 destination
            if (transform.parent != null) {
                // 调用父物体脚本中名为 "OnChildArrived" 的方法，并把自己作为参数传过去
                transform.parent.SendMessage("OnChildArrived", gameObject, SendMessageOptions.DontRequireReceiver);
            }
            
            Debug.Log($"{gameObject.name} 已归位并通知了父物体 {transform.parent.name}");
            }
        }
    }
}