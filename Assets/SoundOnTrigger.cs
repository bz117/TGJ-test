using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // 确保音频不会在一开始就播放
        //audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只要有任何 2D 物体碰到了，这里都会在控制台打印
        Debug.Log("发生了任何形式的碰撞，对方名字是: " + other.gameObject.name);
    }
    
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log("物理碰撞发生了，撞到的是: " + other.name + " 标签是: " + other.tag);
    //     // 检查撞击物是否是我们的声波圆环
    //     // 注意：这里需要确保你的声波预制体 Tag 设为 "Player" 或 "SoundWave"
    //     if (other.CompareTag("SoundWave"))
    //     {
    //         PlayResponseSound();
    //     }
    // }

    void PlayResponseSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            Debug.LogError("函数被调用了！");
            // 调试信息，确认碰撞成功
            Debug.Log($"{gameObject.name} 被声波探测到了！");
            // 如果你希望声音可以重叠（比如多个声波同时撞击），用这个：
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}