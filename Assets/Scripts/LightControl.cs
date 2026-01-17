using UnityEngine;
using UnityEngine.Rendering.Universal; // 必须引用 URP

public class LightTrigger : MonoBehaviour
{
    public Light2D bulbLight;       // 拖入你的 Sprite Light 2D
    public SpriteRenderer spriteRenderer;
    public Sprite LightOn;  // 灯亮后的图片
    public float targetIntensity = 1.2f; // 灯亮后的最终强度
    public float speed = 2.0f;      // 亮起的速度

    private bool isTurningOn = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SoundWave"))
        {
            Debug.Log("检测到碰撞");
            isTurningOn = true;
            spriteRenderer.sprite = LightOn;
            // 如果你想做一次性的触发，可以在这里禁用这个 Collider
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void Update()
    {
        if (isTurningOn)
        {
            // 使用 Lerp 实现平滑的亮度变化
            bulbLight.intensity = Mathf.Lerp(bulbLight.intensity, targetIntensity, Time.deltaTime * speed);
            
            // 当亮度接近目标时，直接设为目标值并停止更新
            if (Mathf.Abs(bulbLight.intensity - targetIntensity) < 0.01f)
            {
                bulbLight.intensity = targetIntensity;
                isTurningOn = false;
            }
        }
    }
}