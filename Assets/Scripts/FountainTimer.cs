using UnityEngine;
using TMPro; // 如果使用 TextMeshPro 显示时间

public class FountainTimer : MonoBehaviour
{
    private float currentTime = 0f;
    private bool isTimerRunning = false;

    public TextMeshProUGUI timerText; // 引用 UI 文本

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 碰到起点：重置并开始计时
        if (other.CompareTag("StartPoint"))
        {
            currentTime = 0f;
            isTimerRunning = true;
            Debug.Log("计时开始！");
        }

        // 碰到终点：停止计时
        if (other.CompareTag("EndPoint") && isTimerRunning)
        {
            isTimerRunning = false;
            Debug.Log("计时结束！最终时间: " + currentTime.ToString("F2") + " 秒");
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "时间: " + currentTime.ToString("F2") + "s";
        }
    }
}