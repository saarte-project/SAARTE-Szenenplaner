using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds a button to the Storyboard. The button adds a new panel to the storyboard.
/// The background panel changes it size, if storyboard panels are added or deleted.
/// </summary>
public class AddButton : MonoBehaviour
{
    #region Init

    public float deltaX;
    public float deltaY;
    public float deltaZ;
    public float expandPanelX;
    public float expandPanelY;
    private int last;
    private GameObject[] panels;
    private int lastCell;
    public int lastCellBackup;
    private Vector3 startPos = new Vector3(-350, -24.5f, 0);
    public GameObject text;
    public GameObject backgroundPanel;
    RectTransform panelRectTransform;
    Vector2 backGroundPanelSize;
    private float initialSizeXBackgroundpanel;
    private float initialSizeYBackgroundpanel;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    void Start()
    {
        transform.localPosition = startPos;
        last = 0;
        panelRectTransform = backgroundPanel.GetComponent<RectTransform>();
        backGroundPanelSize = backgroundPanel.GetComponent<RectTransform>().sizeDelta;
        initialSizeXBackgroundpanel = panelRectTransform.sizeDelta.x;
        initialSizeYBackgroundpanel = panelRectTransform.sizeDelta.y;
    }

    /// <summary>
    /// Restores the initial size of the Panel
    /// </summary>
    public void ResetBackgroundPanel()
    {
        panelRectTransform.sizeDelta = backGroundPanelSize;
    }

    /// <summary>
    /// Add the "Add" button to the last Panel. Called, if a new Panel is created or a Panel has been deleted. Also on App start and loading a file.
    /// </summary>
    public void SetNewPosition()
    {
        lastCell = FindLastCell(GameObject.FindGameObjectsWithTag("Cell"));
        lastCellBackup = lastCell;
        if (lastCell > 4)
        {
            startPos = new Vector3(-350, -210f, 0);
            lastCell = lastCell - 4;
        }
        else
        {
            startPos = new Vector3(-350, -24.5f, 0);
        }
    }

    /// <summary>
    /// Returns the number of the last Panel.
    /// </summary>
    private int FindLastCell(GameObject[] allPanels)
    {
        last = allPanels.Length;
        return last;
    }

    /// <summary>
    /// Keep the Button and the Frame always in the proper position and size
    /// </summary>
    private void Update()
    {
        transform.localPosition = new Vector3((startPos.x + (lastCell * 350f)) + deltaX, startPos.y + deltaY, startPos.z + deltaZ);
        if (lastCellBackup == 3)
        {
            panelRectTransform.sizeDelta = new Vector2(initialSizeXBackgroundpanel + expandPanelX, initialSizeYBackgroundpanel);
        }
        if (lastCellBackup == 4)
        {
            panelRectTransform.sizeDelta = new Vector2(initialSizeXBackgroundpanel + 2 * expandPanelX, initialSizeYBackgroundpanel);
        }
        if (lastCellBackup >= 5)
        {
            panelRectTransform.sizeDelta = new Vector2(initialSizeXBackgroundpanel + 2 * expandPanelX, initialSizeYBackgroundpanel + expandPanelY);
        }
        if (lastCellBackup == 8)
        {
            GetComponent<Image>().enabled = false;
            text.GetComponent<Text>().text = "";
        }
        else
        {
            GetComponent<Image>().enabled = true;
            text.GetComponent<Text>().text = "+";
        }        
    }
}
