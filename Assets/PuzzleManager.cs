using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PassageManager : MonoBehaviour
{
    public static PassageManager Instance;

    [Header("核心设置")]
    public List<int> correctOrder = new List<int> { 0, 1, 2, 3, 4, 5 };
    private List<int> _playerInput = new List<int>();

    [Header("反馈音效")]
    public AudioClip successMusic;
    public AudioClip failureMusic;

    [Header("失败后的处理")]
    public SpriteRenderer blackScreenSprite; 
    public Transform playerTransform;
    public Transform respawnPoint;

    [Header("黑屏时间调整")]
    public float fadeDuration = 0.5f;
    public float stayBlackTime = 1.0f;

    [Header("成功后的处理")]
    // --- 修改点：不再使用 Prefab，改为直接引用场景中的风场物体 ---
    public GameObject windFieldObject; 

    private AudioSource _audioSource;
    private bool _isProcessing = false;

    void Awake() {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        
        if (blackScreenSprite != null) {
            Color c = blackScreenSprite.color;
            c.a = 0;
            blackScreenSprite.color = c;
        }

        // 初始确保风场是关闭的（可选，防止你在编辑器里忘了关）
        if (windFieldObject != null) windFieldObject.SetActive(false);
    }

    public void RecordPassage(int id) {
        if (_isProcessing) return;
        _playerInput.Add(id);
        if (_playerInput.Count == 6) {
            CheckSequence();
        }
    }

    private void CheckSequence() {
        bool isCorrect = _playerInput.SequenceEqual(correctOrder);
        if (isCorrect) {
            HandleSuccess();
        } else {
            StartCoroutine(HandleFailure());
        }
        _playerInput.Clear();
    }

    private void HandleSuccess() {
        if (successMusic != null) {
            _audioSource.PlayOneShot(successMusic);
        }

        // --- 修改点：启用场景中的风场 ---
        if (windFieldObject != null) {
            windFieldObject.SetActive(true);
            Debug.Log("序列正确！风场已开启。");
        } else {
            Debug.LogError("未在 PassageManager 中指定 Wind Field Object！");
        }
    }

    private IEnumerator HandleFailure() {
        _isProcessing = true;
        if (failureMusic != null) _audioSource.PlayOneShot(failureMusic);

        yield return StartCoroutine(FadeBlack(0, 1, fadeDuration));
        yield return new WaitForSeconds(stayBlackTime);

        if (playerTransform != null && respawnPoint != null) {
            playerTransform.position = respawnPoint.position;
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }

        yield return StartCoroutine(FadeBlack(1, 0, fadeDuration));
        _isProcessing = false;
    }

    private IEnumerator FadeBlack(float startAlpha, float endAlpha, float duration) {
        if (blackScreenSprite == null) yield break;
        float elapsed = 0f;
        Color c = blackScreenSprite.color;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            blackScreenSprite.color = c;
            yield return null;
        }
        c.a = endAlpha;
        blackScreenSprite.color = c;
    }
}