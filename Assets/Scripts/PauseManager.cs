using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI 设置")]
    public GameObject mainMenuPanel; 
    public GameObject pauseMenuUI; 
    public GameObject settingsUI; // 建议把设置界面也引用进来

    private void Awake()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 如果设置界面打开着，按 Esc 应该先回到暂停菜单
                if (settingsUI != null && settingsUI.activeSelf)
                {
                    CloseSettings();
                }
                // 如果暂停界面打开着，按 Esc 就关闭它（继续游戏）
                else if (pauseMenuUI.activeSelf)
                {
                    Resume();
                }
                // 如果都关着，就打开暂停界面
                else
                {
                    Pause();
                }
            }
        }else
        {
            mainMenuPanel.SetActive(true);
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(false);
        Time.timeScale = 1f; 
        // 删掉了 isPaused 变量，直接由 UI 状态驱动
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f; //
        //isPaused = false;    //
        
        // 调用 SceneLoader 的异步加载，确保主菜单 UI 能被正确处理
        SceneLoader.Instance.LoadMenu(); //
    }
}