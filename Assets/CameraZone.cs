using UnityEngine;

public class CameraZone : MonoBehaviour
{
    [Header("This Zone Settings")]
    public float zoneCameraSize = 8f; // 该区域特定的镜头大小
    public Vector3 zoneOffset = new Vector3(0, 2, -10); // 该区域特定的偏移

    private CameraFollow camFollow;

    void Start()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 进入时，传给摄像机这个区域专属的数值
            camFollow.UpdateCameraSettings(zoneCameraSize, zoneOffset);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 离开时，恢复默认
            camFollow.ResetToNormal();
        }
    }
}