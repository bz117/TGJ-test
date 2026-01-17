using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Menu; 
    public GameObject Settings;
    
    public void InitializeUI()
    {
        Menu.SetActive(true); 
        Settings.SetActive(false); 
    }

    // menu

    // choose
    public void MenuToSettings() 
    { 
        Menu.SetActive(false); 
        Settings.SetActive(true); 
    }

    public void SettingsToMenu() 
    { 
        Settings.SetActive(false); 
        Menu.SetActive(true); 
    }




}
