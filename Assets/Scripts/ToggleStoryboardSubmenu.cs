using UnityEngine;
using System.Collections;

/// <summary>
/// Activates/deactivates a submenu with contains the two panels Tech/Requisite.
/// </summary>
public class ToggleStoryboardSubmenu : MonoBehaviour {

    #region Init

    public GameObject subMenu;
    public GameObject panelBackground;

    #endregion

    /// <summary>
    /// Start setup
    /// All submenus are closed
    /// </summary>
    private void Start()
    {
        subMenu.SetActive(false);      
    }

    /// <summary>
    /// Toggle the activation/deactivation of the submenu
    /// Button is placed on the top level of the Panel Prefab
    /// </summary>
    public void ToggleMenu()
    {
        if (subMenu.activeInHierarchy)
        {
            subMenu.SetActive(false);
        }
        else
        {          
            StartCoroutine(WaitForSeconds());
        }    
    }

    /// <summary>
    /// Sets the submenu active
    /// Waits until the "ActivateStoryBoardBackgroundPanel" script has performed its actions
    /// </summary>
    private IEnumerator WaitForSeconds()
    {
        yield return new WaitUntil(() => panelBackground.GetComponent<ActivateStoryBoardBackgroundPanel>().GetSyncButtons());
        subMenu.SetActive(true);
    }
}
