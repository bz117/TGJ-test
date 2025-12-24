using UnityEngine;

public class ShiftPassage : MonoBehaviour 
{
    public AudioClip passSound;
    private AudioSource _audioSource;
    private BoxCollider2D _collider;
    private bool _hasPlayedSound = false;
    void Start() {
        _collider = GetComponent<BoxCollider2D>();
            if (_collider == null)
    {
        // 使用 LogError 会在控制台显示红色感叹号，非常醒目
        Debug.LogError($"[检测失败]：{gameObject.name} 物体上没有找到任何 Collider 组件！");
    }
    else
    {
        // 成功获取，打印出这个碰撞体的具体类型（是 BoxCollider 还是 MeshCollider 等）
        Debug.Log($"[检测成功]：已关联到 {gameObject.name} 的 {_collider.GetType().Name}");
    }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
    }

void Update()
    {
        // 检测是否按下了左 Shift 或 右 Shift
        bool isShifting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isShifting)
        {
            // --- 可以通过状态 ---
            _collider.enabled = false; 

            // 播放声音（仅在按下瞬间播放一次）
            if (!_hasPlayedSound && passSound != null)
            {
                _audioSource.PlayOneShot(passSound);
                _hasPlayedSound = true;
            }
        }
        else
        {
            // --- 无法通过状态 ---
            _collider.enabled = true;
            
            // 重置声音标记，以便下次按下时再次播放
            _hasPlayedSound = false;
        }
    }
}