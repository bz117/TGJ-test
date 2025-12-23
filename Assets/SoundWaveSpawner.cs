using UnityEngine;

public class SoundWaveSpawner : MonoBehaviour
{
    public GameObject soundWavePrefab;
    public KeyCode spawnKey = KeyCode.O;

    private SoundWave currentActiveWave; // 记录当前正在生长的圆

    void Update()
    {
        // 1. 刚刚按下：生成圆
        if (Input.GetKeyDown(spawnKey))
        {
            GameObject go = Instantiate(soundWavePrefab, 
            transform.position + new Vector3(0, 60, 0),
            Quaternion.identity);
            currentActiveWave = go.GetComponent<SoundWave>();
        }

        // 2. 持续按住：让圆长大
        if (Input.GetKey(spawnKey) && currentActiveWave != null)
        {
            currentActiveWave.TickGrow();
            // 让圆跟随玩家位置（可选，如果不想要跟随，删掉下面这一行）
            currentActiveWave.transform.position = transform.position;
        }

        // 3. 松开按键：让圆开始消失
        if (Input.GetKeyUp(spawnKey) && currentActiveWave != null)
        {
            currentActiveWave.StartExpiring();
            currentActiveWave = null; // 释放引用，等待下一次按下
        }
    }
}