using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    public GameObject imagePrefab; // 图片预制体
    public float interval = 2.0f;  // 间隔时间（秒）

    void SpawnImage()
    {
        if (imagePrefab != null)
        {
            // 生成图片并获取它的引用
            GameObject newImage = Instantiate(imagePrefab, transform.position, Quaternion.identity);
            
            // 直接告诉 Unity：2秒后把这个新生成的物体删掉
            Destroy(newImage, 2.0f);
        }
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("物理碰撞发生了，撞到的是: " + other.name + " 标签是: " + other.tag);
        // 检查撞击物是否是我们的声波圆环
        // 注意：这里需要确保你的声波预制体 Tag 设为 "Player" 或 "SoundWave"
        if (other.CompareTag("SoundWave"))
        {
            //StartCoroutine(SpawnRoutine());
            SpawnImage();
            PlayResponseSound();
        }
    }

    void PlayResponseSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            // 调试信息，确认碰撞成功
            Debug.Log($"{gameObject.name} 被声波探测到了！");
            // 如果你希望声音可以重叠（比如多个声波同时撞击），用这个：
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}