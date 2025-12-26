using UnityEngine;

public class ShiftPassage : MonoBehaviour 
{
    public AudioClip passSound;
    private AudioSource _audioSource;
    private BoxCollider2D _collider;
    private bool _hasPlayedSound = false;

    void Start() {
        _collider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
    }

    // --- 关键改动 1：使用碰撞持续检测 ---
    void OnCollisionStay2D(Collision2D collision)
    {
        // 只有当碰撞对象是 Player 时才继续
        if (collision.gameObject.CompareTag("Player"))
        {
            // 检测是否按下了 Shift
            bool isShifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (isShifting)
            {
                // 1. 播放声音（利用 bool 确保只响一次）
                if (!_hasPlayedSound && passSound != null)
                {
                    Debug.Log($"[确认碰撞]：正在穿过 {gameObject.name}");
                    _audioSource.PlayOneShot(passSound);
                    _hasPlayedSound = true;
                }

                // 2. 变成触发器，让玩家穿过去
                _collider.isTrigger = true;
            }
        }
    }

    // --- 关键改动 2：离开后重置状态 ---
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _collider.isTrigger = false; // 恢复物理碰撞实体
            _hasPlayedSound = false;     // 重置声音标记
        }
    }
}