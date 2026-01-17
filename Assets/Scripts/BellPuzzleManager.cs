using UnityEngine;
using System.Collections;

public class BellPuzzleManager : MonoBehaviour
{
    [Header("全局配置")]
    public int requiredBells = 4; // 总共需要激活的数量
    
    [Header("受重力物体")]
    public Rigidbody2D targetObjectRb1; // 第一个掉落的钟（2个激活）
    public Rigidbody2D targetObjectRb2; // 第二个掉落的钟（4个激活）

    [Header("阶段 1：激活 2 个钟时")]
    public GameObject windFieldToDestroyStage1; 
    public BoxCollider2D[] windFieldsToResizeStage1; 
    public Vector2 stage1Size = new Vector2(2f, 5f);

    [Header("阶段 2：激活 4 个钟时")]
    public GameObject windFieldToDestroyStage2; // 阶段2要删除的另一个风场
    public BoxCollider2D windFieldToMoveStage2; // 阶段2要改变体积和位置的风场
    public Vector2 stage2Size = new Vector2(5f, 5f); 
    public Vector2 stage2Offset = new Vector2(0f, 2f); 

    [Header("震屏设置")]
    public float shakeDuration = 0.5f; 
    public float shakeMagnitude = 0.2f; 

    private int activatedCount = 0; //
    private bool stage1Triggered = false;
    private bool stage2Triggered = false;

    private void OnEnable()
    {
        BellTrigger.OnBellActivated += HandleBellActivated; //
    }

    private void OnDisable()
    {
        BellTrigger.OnBellActivated -= HandleBellActivated; //
    }

    private void HandleBellActivated()
    {
        activatedCount++; //
        Debug.Log("激活进度: " + activatedCount + "/" + requiredBells); //

        if (activatedCount >= 2 && !stage1Triggered)
        {
            TriggerStage1();
            stage1Triggered = true;
        }

        if (activatedCount >= 4 && !stage2Triggered)
        {
            TriggerStage2();
            stage2Triggered = true;
        }
    }

    private void TriggerStage1()
    {
        if (targetObjectRb1 != null)
        {
            targetObjectRb1.bodyType = RigidbodyType2D.Dynamic; //
        }

        if (windFieldToDestroyStage1 != null)
        {
            windFieldToDestroyStage1.SetActive(false);
        }

        if (windFieldsToResizeStage1 != null)
        {
            foreach (BoxCollider2D col in windFieldsToResizeStage1)
            {
                if (col != null) col.size = stage1Size;
            }
        }
    }

    private void TriggerStage2()
    {
        // 1. 第二个物体掉落
        if (targetObjectRb2 != null)
        {
            targetObjectRb2.bodyType = RigidbodyType2D.Dynamic;
            // 触发震屏
            StartCoroutine(ShakeCamera());
        }

        // 2. 阶段 2 特有的：再删除一个风场
        if (windFieldToDestroyStage2 != null)
        {
            windFieldToDestroyStage2.SetActive(false);
            Debug.Log("阶段 2：第二个指定的风场已消失。");
        }

        // 3. 改变另一个风场的大小和位置
        if (windFieldToMoveStage2 != null)
        {
            windFieldToMoveStage2.size = stage2Size;
            windFieldToMoveStage2.offset = stage2Offset;
        }
    }

    private IEnumerator ShakeCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        Vector3 originalPos = mainCam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCam.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }
}