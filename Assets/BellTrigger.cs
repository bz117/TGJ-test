using UnityEngine;
using System; // 必须引用此命名空间

public class BellTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    private SpriteRenderer _spriteRenderer;
    private AudioSource audioSource;
    
    // 添加一个静态事件，让管理器可以监听
    public static event Action OnBellActivated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTriggered == false && other.CompareTag("SoundWave"))
        {
            SetSpriteAlpha(1.0f); 
            isTriggered = true;
            Debug.Log(gameObject.name + " 已激活");

            // 触发事件，通知管理器
            OnBellActivated?.Invoke();
        }
        else if(isTriggered == true && other.CompareTag("SoundWave"))
        {
            PlayTriggerSound();
        }
    }

    private void PlayTriggerSound()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void SetSpriteAlpha(float alpha)
    {
        Color tempColor = _spriteRenderer.color;
        tempColor.a = alpha;
        _spriteRenderer.color = tempColor;
    }
}