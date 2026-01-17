using UnityEngine;
using UnityEngine.UI;

public class MenuButtonController : MonoBehaviour
{
    public void OnSettingsClick()
    {
        // 直接通过单例调用函数
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OpenSettingsFromMenu();
        }
    }
}