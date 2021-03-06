using UnityEditor;
using UnityEngine;

/// <summary>
/// Example for entry in SAARTE Unity menu
/// </summary>
public class SAARTE_Menu_Entry_Example {
    [MenuItem ("SAARTE/Menu Entry Example", false, 11)]
    static void Example ()
    {
        Debug.Log("SAARTE: This message is displayed in the console when the menu entry is activated.");
    }
}

/// <summary>
/// SAARTE Unity menu entry used for showing information on the project
/// </summary>
public class SAARTE_About_Project {
    [MenuItem ("SAARTE/About SAARTE Project", false, 51)]
    static void About ()
    {
        Debug.Log("SAARTE: Opening SAARTE project website in browser.");
        Application.OpenURL("https://www.hs-worms.de/ux-vis/forschungsbereich/saarte/");
    }
}

/// <summary>
/// SAARTE Unity menu entry used for showing information on the UX-Vis research group
/// </summary>
public class SAARTE_About_UXVis {
    [MenuItem ("SAARTE/About UX-Vis", false, 52)]
    static void About ()
    {
        Debug.Log("SAARTE: Opening developer website in browser.");
        Application.OpenURL("https://www.hs-worms.de/ux-vis/");
    }
}
