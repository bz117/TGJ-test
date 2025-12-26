using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 玩家的 Transform
    
    [Header("Normal View")]
    public float normalSize = 5f;
    public Vector3 normalOffset = new Vector3(0, 0, -10); // 默认偏移

    [Header("Zoom View")]
    public float zoomOutSize = 8f;
    public Vector3 zoomOutOffset = new Vector3(0, 2, -10); // 拉远时的偏移（比如往上抬一点）

    [Header("Settings")]
    public float smoothSpeed = 2f; // 过渡速度

    private Camera cam;
    private float targetSize;
    private Vector3 targetOffset;

    void Start()
    {
        cam = GetComponent<Camera>();
        // 初始化目标值为常规值
        targetSize = normalSize;
        targetOffset = normalOffset;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. 平滑插值计算当前的 Size 和 Offset
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * smoothSpeed);
            Vector3 currentOffset = Vector3.Lerp(transform.position - target.position, targetOffset, Time.deltaTime * smoothSpeed);

            // 2. 更新位置 (跟随玩家 + 当前平滑后的偏移)
            // 注意：这里直接 Lerp 整个位置会更丝滑
            Vector3 desiredPosition = target.position + targetOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }
    }

    // 触发器调用的切换方法
    public void SetZoom(bool isZooming)
    {
        targetSize = isZooming ? zoomOutSize : normalSize;
        targetOffset = isZooming ? zoomOutOffset : normalOffset;
    }
}