using UnityEngine;

public class BirdTrigger : MonoBehaviour
{
    public Transform birds_flock;
    private bool isTriggered = false;
    private SpriteRenderer _spriteRenderer;

    public BirdMove receiver; 
    public Transform destination; 
    public float moveSpeed = 5f;  
    
    private bool shouldMove = false; 

    // --- 新增变量：用于控制玩家 ---
    private GameObject player;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // 自动寻找带有 "Player" 标签的物体
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTriggered == false && other.CompareTag("SoundWave"))
        {
            SetSpriteAlpha(1.0f); 
            if (receiver != null) {
                receiver.RegisterSender(this); 
            }
            isTriggered = true;
        }
    }
    public void OnFinalAction()
    {
        shouldMove = true; 
        // 这里不需要写 SetPlayerControl，父物体在 NotifyAllSenders 时已经锁过了
        Debug.Log(gameObject.name + " 开始向目标移动！");
    }

    void Update() {
        if (shouldMove && destination != null) {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                destination.position, 
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, destination.position) < 0.01f) 
            {
                shouldMove = false;
                transform.SetParent(birds_flock);

                // 物理恢复逻辑... (保持你之前的代码)
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null) { rb.simulated = true; rb.bodyType = RigidbodyType2D.Kinematic; }
                
                // 核心修改：通知父物体该鸟已到达
                if (birds_flock != null) {
                    birds_flock.SendMessage("OnBirdArrived", SendMessageOptions.DontRequireReceiver);
                }
                
                // 这里的 SendMessage("OnChildArrived") 如果你父物体没用到可以保留或删除
                if (transform.parent != null) {
                    transform.parent.SendMessage("OnChildArrived", gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    // --- 新增方法：控制玩家移动开关 ---
    void SetPlayerMovement(bool canMove)
    {
        if (player != null)
        {
            // 方法 A：禁用整个玩家控制脚本（假设脚本叫 PlayerMovement）
            // 注意：请将 "PlayerMovement" 替换为你实际的玩家控制脚本名称
            var moveScript = player.GetComponent("PlayerMovement") as MonoBehaviour;
            if (moveScript != null) moveScript.enabled = canMove;

            // 方法 B：如果是通过 Rigidbody 控制，可以强制清零速度
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (!canMove && playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }
        }
    }

    void SetSpriteAlpha(float alpha)
    {
        Color tempColor = _spriteRenderer.color;
        tempColor.a = alpha;
        _spriteRenderer.color = tempColor;
    }
}