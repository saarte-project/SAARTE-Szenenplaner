using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Buttons cannot be set to Gameobjects, which already have implemented an click-input handler.
/// To create a consistent behaviour for all Panel-elements & Storyboard, this script implements a button functionality.
/// If triggered, the Frame around the Panel activates (highlight the active Panel)
/// </summary>
public class ButtonForNoteAndDropdown : MonoBehaviour, IInputClickHandler {

    #region Init

    private GameObject panelBackground;

    #endregion

    /// <summary>
    /// Get the Frame as a child of the Panel background
    /// </summary>
    void Start () {
        panelBackground = transform.parent.parent.transform.GetChild(0).gameObject;
	}

    /// <summary>
    /// Implements a click input handler, especially for the Hololens by using the "IInputClickHandler"
    /// </summary>
    public virtual void OnInputClicked(InputClickedEventData eventData)
    {
        panelBackground.GetComponent<ActivateStoryBoardBackgroundPanel>().ActivateBackground();
        eventData.Use();
    }
}
