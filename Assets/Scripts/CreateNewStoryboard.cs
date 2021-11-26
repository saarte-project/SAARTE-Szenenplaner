using UnityEngine;

/// <summary>
/// Implements button logic and calls the "ReCreateStartPanels()" function in the "PanelManager" script.
/// If triggered, existing Panels and all dependencys will be destroyed.
/// Two new Panels will be created.
/// </summary>
public class CreateNewStoryboard : MonoBehaviour {

    #region Init

    private GameObject panelListCanvas;

    #endregion

    /// <summary>
    /// Get the Father node, where the "PanelManager" is implemented.
    /// </summary>
    void Start () {
        panelListCanvas = GameObject.FindGameObjectWithTag("PanelListCanvas");             
    }

    /// <summary>
    /// Manages save, load and delete of the storyboard.
    /// Also updates the list of buttons in the menu after something changed.
    /// </summary>
    public void ResetGUI()
    {
        panelListCanvas.GetComponent<PanelManager>().ReCreateStartPanels();
        StaticVariables.fileName = "";
    }
}
