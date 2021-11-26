using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Activates and deactivates the buttons after a mouse click
/// </summary>
public class Selectable : MonoBehaviour, IInputClickHandler
{
    #region Init

    public GameObject buttons;

    #endregion

    /// <summary>
    /// Input click handler for the use with the Hololens
    /// </summary>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        DeselectAll();
        Select();
        eventData.Use();
    }

    /// <summary>
    /// Set button active
    /// </summary>
    public void Select()
    {      
        buttons.SetActive(true);
    }

    /// <summary>
    /// Set all buttons to 'not active'
    /// </summary>
    public static void DeselectAll()
    {    
        GameObject[] allButtons;
        allButtons = GameObject.FindGameObjectsWithTag("Buttons");
        for (int a = 0; a < allButtons.Length; a++)
        {
            allButtons[a].SetActive(false);
        }
    }
}