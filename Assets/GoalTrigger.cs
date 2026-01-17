using UnityEngine;

public class SuccessZone : MonoBehaviour
{
    [Tooltip("这里拖入玩家离开的那个起点喷泉")]
    public FountainChain sourceFountain; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sourceFountain != null)
            {
                sourceFountain.CheckSuccess();
            }
            else
            {
                // 如果没有关联 sourceFountain，则静默处理或报一个友好的警告
                Debug.LogWarning("此触发区域未关联起点喷泉，无法判定挑战。");
            }
        }
    }
}