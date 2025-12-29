using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    // 这里不需要写 Awake 单例，因为它的生命周期由父物体 SceneLoader 管理

    [Header("UI 组件引用 (直接从子物体拖入)")]
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    [Header("音频设置")]
    public AudioMixer mainMixer;
    public string volumeParameterName = "MyExposedVolume"; // 必须与 Mixer 里的 Exposed 名字一致

    private void Start()
    {
        // 游戏启动时，初始化一次滑动条的事件绑定
        if (volumeSlider)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            // 初始化滑块位置（假设你想默认最大音量）
            volumeSlider.value = 0.8f; 
        }

        if (brightnessSlider)
        {
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            brightnessSlider.value = 0f;
        }
    }

    public void SetVolume(float value)
    {
        // 音量转换：0.0001 是为了防止 Log10(0) 出错
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(volumeParameterName, dB);
    }

    public void SetBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            Color tempColor = brightnessOverlay.color;
            // value 建议在 0 到 0.8 之间，否则 1.0 会全黑
            tempColor.a = value; 
            brightnessOverlay.color = tempColor;
        }
    }
}