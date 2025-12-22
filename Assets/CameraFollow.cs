using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 玩家的 Transform
    public Vector3 offset; // 偏移量

    void LateUpdate()
    {
        if(target != null)
        {
            transform.position = target.position + offset;
        }
    }
}