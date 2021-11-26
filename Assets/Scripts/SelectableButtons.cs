using UnityEngine;
using UnityEngine.UI;
using Saarte;

/// <summary>
/// Stores the name of the pressed button in StaticVariables.fileName
/// Changes the color of the pressed button and resets all other buttons
/// </summary>
public class SelectableButtons : MonoBehaviour
{
    #region Init

    private Button button;
    private Color newColor;
    private GameObject holoMessage;
    private GameObject text;

    public string autosaveName;
    public string autosaveFolder;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        holoMessage = GameObject.FindGameObjectWithTag("DebugHoloLens");
        newColor = new Color32(208, 245, 255, 255);
        text = transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// The "Clicked" function is connected and implemented as a part of the Button-Prefeb. (SelectableButton)
    /// </summary>
    public void Clicked()
    {
        StaticVariables.fileName = transform.GetChild(0).GetComponent<Text>().text;
        StaticVariables.selectedAutosaveStoryboard = autosaveFolder;
        StaticVariables.selectedAutosaveName = autosaveName;
        holoMessage.GetComponent<HoloMessage>().setMessage(StaticVariables.saveloadStoryboard_infoText4 + "'" + StaticVariables.fileName + "'" + StaticVariables.saveloadStoryboard_infoText9, 3f);
        GameObject[] allButtons = GameObject.FindGameObjectsWithTag("LoadSaveButton");
        for (int a  = 0; a < allButtons.Length; a++)
        {
            var colors = allButtons[a].GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.pressedColor = Color.white;
            colors.highlightedColor = Color.white;
            allButtons[a].GetComponent<Button>().colors = colors;
            allButtons[a].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            allButtons[a].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
            allButtons[a].transform.GetChild(0).GetComponent<Text>().fontSize = 14;
        }

        button = GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = newColor;
        cb.pressedColor = newColor;
        cb.highlightedColor = newColor;
        button.colors = cb;
        Transform size2 = GetComponent<Transform>();
        size2.localScale = new Vector3(1.2f, size2.localScale.y, size2.localScale.z);
        text.GetComponent<Text>().fontSize = 22;
        text.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    public void SetNameForAutosave(string name)
    {
        autosaveName = name;
    }

    public void SetFolderForAutosave(string name)
    {
        autosaveFolder = name;
    }

    public string GetNameForAutosave()
    {
        return autosaveName;
    }

    public string GetFolderForAutosave()
    {
        return autosaveFolder;
    }
}

