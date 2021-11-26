using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Resets a single Panel to its initial state
/// </summary>
public class ResetStoryboard : MonoBehaviour {

    #region Init

    public GameObject settings;
    public GameObject transitions;
    public GameObject note;
    public GameObject image;
    public Texture2D dummyImage;
    public GameObject panelContainer;
    public GameObject panelContent;
    public GameObject notePrefab;

    #endregion

    /// <summary>
    /// Set all changes back to initial state
    /// </summary>
    public void ResetSinglePanel()
    {
        settings.GetComponent<Dropdown>().value = 0;
        transitions.GetComponent<Dropdown>().value = 0;
        Destroy(note);
        Resources.UnloadUnusedAssets();
        GameObject tempNote = Instantiate(notePrefab, panelContent.transform);
        tempNote.name = "NoticeBackground";
        image.GetComponent<RawImage>().texture = dummyImage;
        image.GetComponent<JPGtoByte>().SetByteArray(dummyImage);
        panelContainer.GetComponent<AddPropsToContainer>().DestroyPropsInSingleContainer(panelContent.GetComponent<CountCells>().GetPanelNumber());
    }  
}
