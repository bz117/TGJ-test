using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Menu; 
    public GameObject Archive; 
    public GameObject Settings;
    
    public void InitializeUI()
    {
        Menu.SetActive(true); 
        Archive.SetActive(false); 
        Settings.SetActive(false); 
    }

    // menu
    public void MenuToArchive() 
    { 
        Menu.SetActive(false); 
        Archive.SetActive(true); 
    }

    // choose
    public void ArichiveToMenu() 
    { 
        Menu.SetActive(true); 
        Archive.SetActive(false); 
    }

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
