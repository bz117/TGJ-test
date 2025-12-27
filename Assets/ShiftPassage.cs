using UnityEngine;

public class ShiftPassage : MonoBehaviour 
{
    public int passageID; // 给每个物体分配一个ID（比如 0 到 5）
    public AudioClip passSound;
    private AudioSource _audioSource;
    private BoxCollider2D _collider;
    private bool _hasPlayedSound = false;

    void Start() {
        _collider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bool isShifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (isShifting)
            {
                if (!_hasPlayedSound)
                {
                    // --- 关键改动：通知管理器 ---
                    PassageManager.Instance.RecordPassage(passageID);
                    
                    if (passSound != null) {
                        _audioSource.PlayOneShot(passSound);
                    }
                    _hasPlayedSound = true;
                }
                _collider.isTrigger = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _collider.isTrigger = false;
            _hasPlayedSound = false; 
        }
    }
}