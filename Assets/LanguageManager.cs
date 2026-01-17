using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageManager : MonoBehaviour
{
    private bool isChanging = false;

    public void ChangeLanguage(int index)
    {
        if (isChanging) return; // 防止连续点击
        StartCoroutine(DoChange(index));
        //Debug.Log("当前语言已切换为: " + LocalizationSettings.SelectedLocale.name);
    }

    private IEnumerator DoChange(int index)
    {
        isChanging = true;
        
        // 关键：等待初始化完成
        yield return LocalizationSettings.InitializationOperation;

        // 检查索引是否合法
        if (index >= 0 && index < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
        
        isChanging = false;
    }
}