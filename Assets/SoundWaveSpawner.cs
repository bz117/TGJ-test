using UnityEngine;

public class SoundWaveSpawner : MonoBehaviour
{
    public GameObject soundWavePrefab;
    public KeyCode spawnKey = KeyCode.O;

    private SoundWave currentActiveWave; 
    private PlayerController playerCtrl; // 引用玩家控制器

    void Start()
    {
        // 缓存引用以提高性能
        playerCtrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        // 获取当前是否在地面上
        bool isGrounded = playerCtrl != null && playerCtrl.isGrounded;

        // 1. 刚刚按下：只有在地面上才能生成圆
        if (Input.GetKeyDown(spawnKey) && isGrounded)
        {
            // 注意：这里的偏移值 (0, 60, 0) 很大，请确保这是你需要的逻辑高度
            GameObject go = Instantiate(soundWavePrefab, 
            transform.position, // 建议初始位置跟随玩家
            Quaternion.identity);
            currentActiveWave = go.GetComponent<SoundWave>();
        }

        // 2. 持续按住：必须在地面上圆才会长大
        if (Input.GetKey(spawnKey) && currentActiveWave != null)
        {
            if (isGrounded)
            {
                currentActiveWave.TickGrow();
                currentActiveWave.transform.position = transform.position;
            }
            else
            {
                // 如果在按住过程中离开地面，强制销毁当前圆
                ForceStopWave();
            }
        }

        // 3. 松开按键：让圆开始消失
        if (Input.GetKeyUp(spawnKey) && currentActiveWave != null)
        {
            ForceStopWave();
        }
    }

    // 统一处理音波消失逻辑
    void ForceStopWave()
    {
        if (currentActiveWave != null)
        {
            currentActiveWave.StartExpiring();
            currentActiveWave = null;
        }
    }
}