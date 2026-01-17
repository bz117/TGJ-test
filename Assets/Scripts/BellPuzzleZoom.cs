using UnityEngine;

public class BellPuzzleZoom : MonoBehaviour
{
    private CameraFollow cameraScript;

    void Start()
    {
        // 找到主摄像机上的脚本
        cameraScript = Camera.main.GetComponent<CameraFollow>();
    }
}