using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    [Header("Default View")]
    public float normalSize = 5f;
    public Vector3 normalOffset = new Vector3(0, 0, -10);

    [Header("Settings")]
    public float smoothSpeed = 2f; 

    private Camera cam;
    private float targetSize;
    private Vector3 targetOffset;

    void Start()
    {
        cam = GetComponent<Camera>();
        // 初始状态为默认视角
        targetSize = normalSize;
        targetOffset = normalOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;
        // 使用 Lerp 时，建议对 Time.deltaTime * smoothSpeed 进行限制，防止卡顿瞬间数值过大
        float t = Mathf.Clamp01(Time.deltaTime * smoothSpeed);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, t);
        
        Vector3 desiredPosition = target.position + targetOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
    }

    // 修改：现在可以接收具体的 size 和 offset 参数
    public void UpdateCameraSettings(float size, Vector3 offset)
    {
        targetSize = size;
        targetOffset = offset;
    }

    // 恢复默认视角
    public void ResetToNormal()
    {
        targetSize = normalSize;
        targetOffset = normalOffset;
    }
}