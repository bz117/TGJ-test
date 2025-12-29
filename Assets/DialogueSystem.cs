using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DialogueSystem : MonoBehaviour
{
    // 在 Inspector 面板里直接拖入对应的 String Reference
    public LocalizedString pickUpMessage = new LocalizedString { TableReference = "GameText", TableEntryReference = "pickup_item" };

    public void ShowPickUpTip()
    {
        // 自动根据当前语言异步获取文本
        pickUpMessage.GetLocalizedStringAsync().Completed += handle => {
            Debug.Log("获取到的文本是: " + handle.Result);
            // 这里可以赋值给你的 UI 或 对话框
        };
    }
}