using UnityEngine;

/// <summary>
/// All static variables in one place in one singleton.
/// The language is automatically selected by the system language
/// </summary>
public class StaticVariables : MonoBehaviour
{
    #region Init

    public static Vector3 maxSize = new Vector3(1, 1, 1);
    public static Vector3 minSize = new Vector3(1f, 1f, 1f);
    public static Vector3 propsMenuButtonsSize = new Vector3(0.5f, 0.5f, 0.5f);
    public static Vector3 maxSizeProps = new Vector3(1f, 1f, 1f);
    public static Vector3 minSizeProps = new Vector3(0, 0, 0);
    public static float rigidAngularDrag = 0.05f;
    public static float rotationSensitivity = 1000.0f;
    public static string textRotateMod = "Modus: Object rotieren";
    public static string textMoveMod = "Modus: Object bewegen";
    public static string textRotateChildMod = "Modus: Child Object rotieren";
    public static string nameButtonRotate = "ButtonRotate";
    public static string nameButtonMove = "ButtonMove";
    public static string objectTag = "AVP_Object";
    public static string objectTagUndefinedState = "AVP_Object_Moving";
    public static string nameButtonRotateChild = "ButtonRotateChild";
    public static string objectGlobalScaler = "GlobalScaler";
    public static string savedFilepath;
    public static string fileName = "Unbenannt";
    public static string activeStoryBoard = "Unbenannt";
    public static string selectedAutosaveStoryboard = "";
    public static string selectedAutosaveName = "";

    #endregion

    #region FileManager

    public static string ownMessage = "Singleton for Accessing Static Names AWAKE";
    public static string saveloadStoryboard_infoText1 = "Bitte einen Namen eingeben.";
    public static string saveloadStoryboard_infoText2 = " wurde erfolgreich gespeichert.";
    public static string saveloadStoryboard_infoText3 = "Bitte wählen Sie zuerst ein Storyboard aus.";
    public static string saveloadStoryboard_infoText4 = "Das Storyboard ";
    public static string saveloadStoryboard_infoText5 = " wurde gelöscht.";
    public static string saveloadStoryboard_infoText6 = " existiert nicht.";
    public static string saveloadStoryboard_infoText7 = "Storyboard konnte nicht geladen werden.";
    public static string saveloadStoryboard_infoText8 = "Storyboard wurde geladen.";
    public static string saveloadStoryboard_infoText9 = " wurde ausgewählt.";

    #endregion

    private static StaticVariables instance = null;

    /// <summary>
    /// The Singleton Instance for the Static Class
    /// </summary>
    public static StaticVariables Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Changes the savedFilePath
    /// </summary>
    private void Awake()
    {
        savedFilepath = Application.persistentDataPath + "/Saves/";

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}