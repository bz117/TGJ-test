using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // 单例实例（全局唯一访问点）
    public static SceneLoader Instance { get; private set; }

    [Header("UI 引用（必须赋值）")]
    public CanvasGroup fadeCanvasGroup; // 淡入淡出的画布组
    public Canvas pauseCanvas; // 暂停面板
    public GameObject mainMenuPanel; // 主菜单面板
    public GameObject settingsPanel; // 设置面板

    [Header("渐变设置")]
    public float fadeDuration = 1f; // 渐变时长（秒）

    // 记录设置面板的打开来源
    private enum SettingsSource { None, Menu, Pause }
    private SettingsSource currentSettingsSource = SettingsSource.None;

    // 判断是否正在渐变
    private bool isFading => fadeCanvasGroup != null && fadeCanvasGroup.alpha > 0f && fadeCanvasGroup.alpha < 1f;

    private void Awake()
    {
        // 单例初始化：确保全局唯一，切换场景不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 清理多余的EventSystem（避免UI交互冲突）
        var eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystems.Length > 1)
        {
            foreach (var es in eventSystems)
            {
                if (es.gameObject != gameObject)
                    Destroy(es.gameObject);
            }
        }
    }

    private void Start()
    {
        // 初始进入菜单场景时，初始化UI
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            InitializeMenuUI();
        }
    }

    private void Update()
    {
        // ESC键触发暂停（非菜单场景、非渐变中）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != "Menu" && !isFading)
            {
                TogglePause();
            }
        }
    }

    // ====== 对外暴露的场景加载方法 ======
    // 加载菜单场景
    public void LoadMenu()
    {
        LoadSceneWithFade("Menu");
    }

    // 加载First场景
    public void LoadFirst()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        LoadSceneWithFade("First");
    }

    // 加载Second1场景
    public void LoadSecond1()
    {
        LoadSceneWithFade("Second1");
    }

    // 加载Second2场景
    public void LoadSecond2()
    {
        LoadSceneWithFade("Second2");
    }

    // 加载Second3场景
    public void LoadSecond3()
    {
        LoadSceneWithFade("Second3");
    }

    // 退出游戏
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ====== UI交互方法 ======
    // 从菜单打开设置面板
    public void OpenSettingsFromMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        currentSettingsSource = SettingsSource.Menu;
    }

    // 暂停/继续游戏
    public void TogglePause()
    {
        if (pauseCanvas == null) return;

        bool isPaused = pauseCanvas.gameObject.activeSelf;
        pauseCanvas.gameObject.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1f : 0f; // 暂停时时间停止
    }

    // 从暂停面板打开设置
    public void OpenSettingsFromPause()
    {
        if (pauseCanvas != null && settingsPanel != null)
        {
            pauseCanvas.gameObject.SetActive(false);
            settingsPanel.SetActive(true);
            currentSettingsSource = SettingsSource.Pause;
        }
    }

    // 关闭设置面板
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // 根据打开来源，回到对应界面
        switch (currentSettingsSource)
        {
            case SettingsSource.Menu:
                if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                break;
            case SettingsSource.Pause:
                if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(true);
                break;
            default:
                if (SceneManager.GetActiveScene().name == "Menu" && mainMenuPanel != null)
                    mainMenuPanel.SetActive(true);
                break;
        }

        currentSettingsSource = SettingsSource.None;
    }

    // ====== 核心场景加载逻辑 ======
    // 带淡入淡出的场景加载（对外调用）
    public void LoadSceneWithFade(string sceneName)
    {
        if (isFading || fadeCanvasGroup == null) return;
        StartCoroutine(LoadSceneWithFadeRoutine(sceneName));
    }

    // 场景加载协程（内部逻辑）
    private IEnumerator LoadSceneWithFadeRoutine(string sceneName)
    {
        // 隐藏当前场景的UI
        HideCurrentSceneUI();

        // 淡出（屏幕变黑）
        yield return FadeTo(1f);

        currentSettingsSource = SettingsSource.None;
        Time.timeScale = 1f; // 恢复时间正常

        // 异步加载场景（避免卡顿）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 暂停自动激活
        while (asyncLoad.progress < 0.9f) yield return null; // 等待加载到90%
        yield return new WaitForSecondsRealtime(0.5f);
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // 初始化新场景UI
        if (sceneName == "Menu")
        {
            InitializeMenuUI();
        }
        else
        {
            if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        // 淡入（屏幕变亮）
        yield return FadeTo(0f);
    }

    // 隐藏当前场景的所有UI
    private void HideCurrentSceneUI()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Menu")
        {
            var mainMenu = GameObject.Find("MainMenuPanel");
            var settings = GameObject.Find("SettingsPanel");
            var pause = GameObject.Find("PauseCanvas");
            if (mainMenu != null) mainMenu.SetActive(false);
            if (settings != null) settings.SetActive(false);
            if (pause != null) pause.SetActive(false);
        }
        else
        {
            var pause = GameObject.Find("PauseCanvas");
            var settings = GameObject.Find("SettingsPanel");
            if (pause != null) pause.SetActive(false);
            if (settings != null) settings.SetActive(false);
        }
    }

    // 初始化菜单场景UI
    private void InitializeMenuUI()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    // 淡入淡出渐变逻辑
    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime; // 不受暂停影响
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
    }
}