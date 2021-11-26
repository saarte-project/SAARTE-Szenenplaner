using UnityEngine;

/// <summary>
/// If enabled, the script performes an autosave. It calls the function 'SaveStoryBoard' in the 'SaveLoadStoryboard' script.
/// </summary>
public class AutoSaveStoryboard : MonoBehaviour {

    #region Init

    [Tooltip("The time in which the Storyboard gets saved automatically.")]
    public float autoSaveTime;
    [Tooltip("Activates or deactivates the Autosave functionality.")]
    public bool activateAutoSave;
    public float elapsed = 0f;

    #endregion

    /// <summary>
    /// If all conditions are set to truer in Update method, the Autosave starts
    /// </summary>
    void Update()
    {
        if (activateAutoSave && StaticVariables.activeStoryBoard != "Unbenannt" && (!StaticVariables.activeStoryBoard.Contains("Autosave")))
        {
            elapsed += Time.deltaTime;
            if (elapsed >= autoSaveTime)
            {
                elapsed = 0;
                StartAutoSave();
            }
        }
    }

    /// <summary>
    /// Save the Storyboard periodically
    /// </summary>
    void StartAutoSave()
    {
        GetComponent<FileManager>().OnAutoSaveClick();
    }
}
