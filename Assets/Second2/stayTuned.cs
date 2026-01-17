using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    [Tooltip("当触发时要启用的物体")]
    public GameObject targetObject;

    [Tooltip("可选：只允许特定标签的对象触发（例如 'Player'）")]
    public string requiredTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        if (targetObject != null)
            targetObject.SetActive(true);
    }
}