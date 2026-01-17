using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("配置选项")]
    public GameObject imagePrefab; // 图片预制体
    public AudioClip soundEffect;  // 音效文件
    public float interval = 2.0f;  // 间隔时间（秒）

    private AudioSource audioSource;

    void Start()
    {
        // 获取或添加音效组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 启动定时生成协程
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1. 在当前物体位置生成图片
            SpawnImage();

            // 2. 播放音效
            if (soundEffect != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }

            // 3. 等待指定的时间
            yield return new WaitForSeconds(interval);
        }
    }

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
}