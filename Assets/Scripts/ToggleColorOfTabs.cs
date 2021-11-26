using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AFter clicking on a Tab on an active panel, the color changes to visualize the activation of the tab
/// </summary>
public class ToggleColorOfTabs : MonoBehaviour
{
    #region Init

    private Button button;
    private Color newColor;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        newColor = new Color32(208, 245, 255, 255);
    }

    /// <summary>
    /// Toggles the color of the buttons
    /// </summary>
    public void ToggleColor()
    {
        GameObject[] allButtons = GameObject.FindGameObjectsWithTag("Tabs");
        for (int a = 0; a < allButtons.Length; a++)
        {
            /* 
             * Change Color of all Buttone to White
             */

            var colors = allButtons[a].GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.pressedColor = Color.white;
            colors.highlightedColor = Color.white;
            allButtons[a].GetComponent<Button>().colors = colors;
        }

        /* 
         * Change Color of all Buttone to blue
         */

        button = GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = newColor;
        cb.pressedColor = newColor;
        cb.highlightedColor = newColor;
        button.colors = cb;
    }
}
