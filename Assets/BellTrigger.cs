using UnityEngine;

public class BellTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    private SpriteRenderer _spriteRenderer;
    private AudioSource audioSource;
    
    // 当发生碰撞时触发（物体必须有 Collider，且至少一个有 Rigidbody）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTriggered == false && other.CompareTag("SoundWave"))
        {
            // 检查碰撞对象的标签（可选，避免任何东西碰到都变色）
            SetSpriteAlpha(1.0f); 
            isTriggered = true;
            Debug.Log("监测到碰撞");
        }else
        {
            PlayTriggerSound();
        }
    }

    private void PlayTriggerSound()
    {
        if (audioSource != null)
        {
            AudioClip collisionSound = audioSource.clip;
            Debug.Log(gameObject.name + " 音频获取成功");
            audioSource.PlayOneShot(collisionSound);
            Debug.Log(gameObject.name + " 发出了声音");
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void SetSpriteAlpha(float alpha)
    {
        // 不能直接修改 _spriteRenderer.color.a，必须先取出来赋值给临时变量
        Color tempColor = _spriteRenderer.color;
        tempColor.a = alpha;
        _spriteRenderer.color = tempColor;
    }
}