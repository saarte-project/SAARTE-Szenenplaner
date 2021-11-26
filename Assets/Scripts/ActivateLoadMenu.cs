using UnityEngine;

/// <summary>
/// Turns Storyboard menu on and off
/// Buttons are available on the Storyboard Menu and the Storyboard itself
/// </summary>
public class ActivateLoadMenu : MonoBehaviour
{
    #region Init

    public GameObject menu;
    public GameObject storyboardMenu;

    #endregion

    /// <summary>
    /// On startup, the menu should not be visible
    /// </summary>
    public void InitializeDeactivated()
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// The open/close menu functionality can get accessed via two buttons. This function is to toggle the on/off state
    /// </summary>
    public void ToggleMenu()
    {       
        if (menu.activeInHierarchy)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
            storyboardMenu.GetComponent<FileManager>().UpdateAllFiles();
        }
        StaticVariables.fileName = "";
    }
}
    
