using UnityEngine;

/// <summary>
/// Turn on and off the debug info in the scene.
/// Connected to a button in the scene
/// Set to false at start
/// Can be deleted if no debug info is needed anymore
/// </summary>
public class ActivateDebugInfo : MonoBehaviour {

    #region Init

    public GameObject debugInfo;

    #endregion

    /// <summary>
    /// Do something on Startup 
    /// </summary>
    private void Start()
    {
        debugInfo.SetActive(false);
    }

    /// <summary>
    /// Toggles the Debug Info on a button click
    /// </summary>
    public void ToggleDebugInfo()
    {
        if (debugInfo.activeInHierarchy)
        {
            debugInfo.SetActive(false);
        }
        else
        {
            debugInfo.SetActive(true);
        }
    }
}

