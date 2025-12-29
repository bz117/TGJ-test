using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI Canvas 引用")]
    public GameObject mainMenuPanel;   // 新增：拖入主菜单的 Panel 或 Canvas
    public Canvas pauseCanvas;    // 拖入你的 Pause Canvas
    public Canvas settingsCanvas; // 拖入你的 Settings Canvas

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // 结束执行，防止逻辑冲突
        }

        // 每一帧或每次场景加载后检查是否有重复的 EventSystem
        SceneManager.sceneLoaded += (scene, mode) => {
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length > 1)
            {
                foreach (var es in eventSystems)
                {
                    // 如果这个 EventSystem 不是我带过来的那个，就删掉它
                    if (es.transform.parent != transform) 
                    {
                        Destroy(es.gameObject);
                    }
                }
            }
        };
    }
    void Update()
    {
        // 排除主菜单场景（假设主菜单名为 "Menu"）
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    #region 核心业务逻辑

    public void PauseGame()
    {
        isPaused = true;
        pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f; // 冻结物理和时间
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseCanvas.gameObject.SetActive(false);
        if (settingsCanvas) settingsCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f; // 恢复时间
    }

    // 打开设置界面（通常在 Pause 界面点击 "Settings" 按钮时触发）
    public void OpenSettings()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        // 如果在游戏里打开设置，隐藏暂停界面
        // 如果在主菜单，pauseCanvas 可能是空的或不需要处理
        if (pauseCanvas != null) 
        {
            pauseCanvas.gameObject.SetActive(false);
        }
        
        // 打开设置界面
        if (settingsCanvas != null) 
        {
            settingsCanvas.gameObject.SetActive(true);
        }
    }
    // 返回暂停界面（在 Settings 界面点击 "Return" 按钮时触发）
    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true); // 回到主菜单
        }
        else
        {
            if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(true); // 回到暂停界面
        }
    }    

    #endregion

    #region 异步加载
    public void LoadLevel_01()
    {
        //ResumeGame();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false); // 关闭主菜单
        Time.timeScale = 1f;
        StartCoroutine(LoadYourAsyncScene("Level_01"));
    }

// 4. 返回主菜单
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);
        // 异步加载 Menu 场景
        StartCoroutine(LoadYourAsyncScene("Menu"));
        // 注意：mainMenuPanel 会随 GlobalManager 跨场景，
        // 加载完成后可以在 OnSceneLoaded 里确保它被激活
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;
        
        // 加载完成后，如果是回主菜单，确保菜单显示
        if (sceneName == "Menu" && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
    #endregion
}