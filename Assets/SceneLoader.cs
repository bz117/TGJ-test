using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//异步加载场景防止加载时游戏画面卡顿
public class SceneLoader : MonoBehaviour
{
    public void LoadLevel_01()
    {
        StartCoroutine(LoadYourAsyncScene("Level_01"));
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadYourAsyncScene("Menu"));
    }

    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // 启动异步加载
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // 加载未完成时持续执行
        while(!asyncLoad.isDone)
        {
            // 获取加载进度
            float progress = asyncLoad.progress; // 可以在UI上显示这个进度条
            yield return null; // 等待下一帧
        }
    }
}