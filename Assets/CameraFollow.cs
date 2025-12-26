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

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * smoothSpeed);
        Vector3 desiredPosition = target.position + targetOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
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