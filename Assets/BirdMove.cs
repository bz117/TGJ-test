using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BirdMove : MonoBehaviour
{
    private int arrivedCount = 0; // 更清晰的变量名
    public GameObject bird1;
    public GameObject bird2;
    public GameObject bird3;
    public GameObject bird4;
    public GameObject bridge;

    private HashSet<BirdTrigger> activeSenders = new HashSet<BirdTrigger>();
    public int targetCount = 4; 
    // private int isBack = 0;
    public Transform destination; 
    public float moveSpeed = 5f;  

    // [Header("视觉与碰撞设置")]
    // public SpriteRenderer targetSprite; // 最终要显示的正常图片
    // public SpriteRenderer whiteMask;   // 覆盖在上面的白色遮罩图片
    // public float fadeSpeed = 2f;       // 遮罩透明度变化速度
    // public float stayWhiteDuration = 2f; // 白色全显后的停留时间

    [Header("玩家控制")]
    public string playerMovementScriptName = "PlayerMovement"; 
    private GameObject player;
    private bool isPlayerLocked = false;
    private bool effectStarted = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        arrivedCount = 0;
        // 初始状态设置
        // if (targetSprite != null)
        // {
        //     // 目标图片初始完全透明
        //     SetAlpha(targetSprite, 0);
        //     // 初始关闭碰撞体积
        //     Collider2D col = targetSprite.GetComponent<Collider2D>();
        //     if (col != null) col.enabled = false;
        // }

        // if (whiteMask != null)
        // {
        //     // 白色遮罩初始完全透明，颜色确保是纯白
        //     whiteMask.color = new Color(1, 1, 1, 0);
        // }
    }

    // 新增：供 BirdTrigger 调用的方法
    public void ReportBirdArrived()
    {
        arrivedCount++;
        Debug.Log($"小鸟已到达: {arrivedCount}/{targetCount}");

        if (arrivedCount >= targetCount)
        {
            // 所有小鸟都到了，开始执行最终移动
            // 注意：这里不再需要 activeSenders 集合了
        }
    }

    public void RegisterSender(BirdTrigger sender)
    {
        if (!activeSenders.Contains(sender))
        {
            activeSenders.Add(sender);
            if (activeSenders.Count >= targetCount)
            {
                SetPlayerControl(false); 
                //NotifyAllSenders();
            }
        }
    }

    // void NotifyAllSenders()
    // {
    //     foreach (BirdTrigger s in activeSenders)
    //     {
    //         s.OnFinalAction();
    //     }
    // }

    // public void OnChildArrived(GameObject child)
    // {
    //     isBack += 1;
    // }

    void Update() 
    {
        // 现在只需要检查 arrivedCount
        if (arrivedCount >= targetCount && !effectStarted)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, destination.position) < 0.01f)
            {
                effectStarted = true;
                bridge.SetActive(true);
                if (isPlayerLocked) 
                {
                    SetPlayerControl(true);
                }
            }
        }
    }
    // IEnumerator MaskTransitionSequence()
    // {
    //     if (targetSprite == null || whiteMask == null) yield break;

    //     // --- 1. 白色遮罩透明度由 0 变 1 ---
    //     while (whiteMask.color.a < 1f)
    //     {
    //         SetAlpha(whiteMask, Mathf.MoveTowards(whiteMask.color.a, 1f, Time.deltaTime * fadeSpeed));
    //         yield return null;
    //     }

    //     // --- 2. 在全白状态下停留 2 秒 ---
    //     bird1.SetActive(false);
    //     bird2.SetActive(false);
    //     bird3.SetActive(false);
    //     bird4.SetActive(false);
    //     yield return new WaitForSeconds(stayWhiteDuration);

    //     // --- 3. 瞬间显示下方目标图片，并开启碰撞 ---
    //     SetAlpha(targetSprite, 1f);
    //     Collider2D col = targetSprite.GetComponent<Collider2D>();
    //     if (col != null) 
    //     {
    //         col.enabled = true;
    //         col.isTrigger = false;
    //     }

    //     // --- 4. 白色遮罩透明度由 1 变 0 ---
    //     while (whiteMask.color.a > 0f)
    //     {
    //         SetAlpha(whiteMask, Mathf.MoveTowards(whiteMask.color.a, 0f, Time.deltaTime * fadeSpeed));
    //         yield return null;
    //     }

    //     // --- 5. 恢复玩家控制 ---
    //     if (isPlayerLocked)
    //     {
    //         SetPlayerControl(true);
    //     }
    // }

    // 快捷修改透明度的辅助方法
    // void SetAlpha(SpriteRenderer renderer, float alpha)
    // {
    //     Color c = renderer.color;
    //     c.a = alpha;
    //     renderer.color = c;
    // }

    void SetPlayerControl(bool canMove)
    {
        if (player == null) return;
        var moveScript = player.GetComponent(playerMovementScriptName) as MonoBehaviour;
        if (moveScript != null) moveScript.enabled = canMove;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && !canMove) rb.velocity = Vector2.zero;
        isPlayerLocked = !canMove; 
    }
}