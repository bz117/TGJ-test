using UnityEngine;

public class ZoomTrigger : MonoBehaviour
{
    private CameraFollow cameraScript;

    void Start()
    {
        // 找到主摄像机上的脚本
        cameraScript = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraScript.SetZoom(true); // 玩家进入，拉远
            Debug.Log("开始拉远镜头");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraScript.SetZoom(false); // 玩家离开，恢复
            Debug.Log("开始恢复镜头");
        }
    }
}