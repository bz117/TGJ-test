using UnityEngine;

// 主循环
public class GameManager : MonoBehaviour
{
    public UIManager uiManager; 

    void Start()
    {
        uiManager.InitializeUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}