using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("场景切换配置")]
    [Tooltip("目标场景名称（必须和Build Settings一致）")]
    public string targetSceneName = "Second1"; // 默认切换到Second1

    // 触发器检测：玩家进入时切换场景
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是玩家触发（标签必须和编辑器一致，大小写敏感）
        if (other.CompareTag("Player"))
        {
            Debug.Log($"检测到玩家，准备切换到场景：{targetSceneName}");

            // 校验SceneLoader实例是否存在
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("SceneLoader实例不存在！请检查菜单场景中是否有SceneLoader对象");
                return;
            }

            // 根据目标场景名称调用对应加载方法
            switch (targetSceneName)
            {
                case "Menu":
                    SceneLoader.Instance.LoadMenu();
                    break;
                case "First":
                    SceneLoader.Instance.LoadFirst();
                    break;
                case "Second1":
                    SceneLoader.Instance.LoadSecond1();
                    break;
                case "Second2":
                    SceneLoader.Instance.LoadSecond2();
                    break;
                default:
                    // 通用加载（支持新增场景）
                    SceneLoader.Instance.LoadSceneWithFade(targetSceneName);
                    break;
            }
        }
    }
}