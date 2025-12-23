using UnityEngine;

public class FountainMove : MonoBehaviour
{
    public GameObject targetObject; 
    public float targetPosition = 2000;
    public float speed = 2f;

    private bool shouldMove = false;

    // 关键修正：必须使用 2D 版本的触发函数
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // 确保你的玩家物体 Tag 设置为 "Player"
        if (other.CompareTag("Player"))
        {
            shouldMove = true;
            Debug.Log("2D玩家已触发！");
        }
    }

    private void Update()
    {
        if (shouldMove && targetObject != null)
        {
            targetObject.transform.position += new Vector3(0, speed, 0);
        }
        if(targetObject.transform.position.y >= targetPosition)
        {
            shouldMove = false;
        }
    }
}