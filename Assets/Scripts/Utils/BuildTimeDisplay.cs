using UnityEngine;

/// <summary>
/// This class loads and displays the time at which application has been built.
/// </summary>
public class BuildTimeDisplay : MonoBehaviour {
    public const string fileName = "SAARTE_buildTime"; //!< Hardcoded filename without suffix for storing the build time.
    string buildTime = "empty"; //!< Stores thes build time as read from file.
    
    /// <summary>
    /// Initially reading the built time and creating the gameObject to display it in the HoloLens scene
    /// </summary>
    void Start () {
        // Reading build time from file.
        TextAsset txtAsset = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        buildTime = txtAsset.text;
        
        GameObject buildTimeDisplayObject = GameObject.Find("BuildTimeText");
        buildTimeDisplayObject.GetComponent<UnityEngine.UI.Text>().text = "Build time: "+ buildTime;
        
    }

    /// <summary>
    /// Display built time in unity editor GUI.
    /// </summary>
    public void OnGUI()
    {
        // This is visible in unity play mode only
        GUILayout.Label("Build time: " + buildTime);        
    }
}
