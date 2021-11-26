using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all important functionalities of each panel
/// Recalculates the amount of panels and writes that information to the Storyboard
/// Destroys a storyboard after clicking the remove button
/// </summary>
public class PanelManager : MonoBehaviour {

    #region init

    public GameObject resizableWindow;
    public GameObject addPanelButton;
    public RectTransform panel;
    public RectTransform backgroundPanel;
    private int numberOfPanels;
    private GameObject [] allPanels;
    private float cellsizeX, cellSizeY;
    public  float distancePanel;
    public  float distanceButton;
    private Vector2 panelOriginalSizeDelta;
    private Vector2 panelOriginalPosition;
    public int resizableWindowOriginalPaddingRight;
    private Vector2 addPanelButtonOriginalPosition;
    private Vector2 backGroundPanelOriginalSizeDelta;
    private Vector2 backGroundPanelOriginalPosition;
    public GameObject cellClone;
    public GameObject panelContent;
    public GameObject panelClone;
    public GameObject panelAnchor;
    public GameObject storyBoardLabel;

    #endregion

    void Start ()
    {
        gameObject.AddComponent<DestroyAllProps>();
        CreateStartPanels(2);
        addPanelButtonOriginalPosition = addPanelButton.GetComponent<RectTransform>().localPosition;
    }

    /// <summary>
    /// Instantiates a new panel 
    /// </summary>
    public void OnAddButtonPressed()
    {
        numberOfPanels = GameObject.FindGameObjectsWithTag("Cell").Length;
        if (numberOfPanels < 8)
        {
            /*
             * Instantiate new Panel
             */

            GameObject tempCell =  Instantiate(cellClone, resizableWindow.transform);
            tempCell.SetActive(true);
            tempCell.transform.tag = "Cell";
            GameObject.FindGameObjectWithTag("AddPanelButton").GetComponent<AddButton>().SetNewPosition();
            AddInitialByteArrayToImage(tempCell);

            /*
             * Shrink all Props to make them invisible for the User after changing the Panel
             */

            if (gameObject.GetComponent<DestroyAllProps>())
            {
                gameObject.GetComponent<DestroyAllProps>().ToggleAllPropsInScene(new GameObject(), false);
            }
        }    
    }

    /// <summary>
    /// Setup for a new Storyboard
    /// </summary>
    public void OnLoadButtonPressed()
    {
        InitializeAddButton();
        ReorganizeStoryboard();
    }

    /// <summary>
    /// Moves the Add button and the BackgroundPanel
    /// </summary> 
    private void ReorganizeStoryboard()
    {
        GameObject.FindGameObjectWithTag("AddPanelButton").GetComponent<AddButton>().SetNewPosition();
        GameObject.FindGameObjectWithTag("AddPanelButton").GetComponent<AddButton>().ResetBackgroundPanel();
    }

    /// <summary>
    /// Removes the panel and recalculates the panel number for each panel
    /// </summary>
    /// <param name="panelToDestroy">The panel that has to be destroyed</param>
    public void OnRemoveButtonPressed(GameObject panelToDestroy)
    {
        /*
         * Destroy a single Panel and recalculate the numbering of the panels
         */

        numberOfPanels = GameObject.FindGameObjectsWithTag("Cell").Length;      
        RecalculatePanels(panelToDestroy);     
    }

    /// <summary>
    /// Sets the add button back to original position
    /// </summary>
    public void InitializeAddButton()
    {       
        addPanelButton.GetComponent<RectTransform>().localPosition = addPanelButtonOriginalPosition;
    }

    /// <summary>
    /// Creates start panels for the storyboard
    /// </summary>
    public void CreateStartPanels(int nr)
    {     
        CreateNewPanels(nr);        
    }

    /// <summary>
    /// Creates an initial setup for the Storyboard and the panels
    /// </summary>
    public void ReCreateStartPanels()
    {
        DestroyPropsInSpawnContainer();
        DestroyAllPanels();
        CreateNewPanels(2);
    }

    /// <summary>
    /// Instantiates new Panels for the storyboard
    /// </summary>
    /// <param name="number">Amount of  panels to create</param>
    public void CreateNewPanels(int number)
    {
        for (int x = 0; x < number; x++)
        {
            GameObject[] n = new GameObject[x + 1];
            n[x] = Instantiate(panelClone, panelAnchor.transform);
            n[x].transform.tag = "Cell";
            n[x].SetActive(true);
            AddInitialByteArrayToImage(n[x]);
        }
        StaticVariables.fileName = "Unbenannt";
        StaticVariables.activeStoryBoard = "Unbenannt";
        storyBoardLabel.GetComponent<Text>().text = "Storyboard: Unbenannt";
        StartCoroutine(WaitAndNumeratePanel());
    }

    /// <summary>
    /// Initial creation of the byte array. We need this to avoid a crash if the user saves the storyboard and no picture was taken before.
    /// </summary>
    /// <param name="clonedCell">The panel</param>
    private void AddInitialByteArrayToImage(GameObject clonedCell)
    {
        Transform panelPicture = clonedCell.transform.GetChild(1).GetChild(0);
        panelPicture.GetComponent<JPGtoByte>().SetByteArray(panelPicture.GetComponent<RawImage>().texture as Texture2D);
    }

    /// <summary>
    /// Destroys the panel, moves the add button and recalculates the panel number
    /// </summary>
    /// <param name="panelToDestroy">The panel to destroy</param>
    public void RecalculatePanels(GameObject panelToDestory)
    {
        StartCoroutine(DestroySinglePanelAndRecalculatePanelNumbers(panelToDestory));
    }

    /// <summary>
    /// Destroys the panel, moves the add button and recalculates the panel number
    /// </summary>
    /// <param name="panelToDestroy">The panel to destroy</param>
    private IEnumerator DestroySinglePanelAndRecalculatePanelNumbers(GameObject panelToDestroy)
    {
        Destroy(panelToDestroy);
        yield return new WaitForSeconds(0.5f);
        allPanels = GameObject.FindGameObjectsWithTag("Cell");
        for (int a = 0; a < allPanels.Length; a++)
        {
            allPanels[a].GetComponentInChildren<CountCells>().SetPanelNumber(a + 1);
        }
        ReorganizeStoryboard();
        yield return null;
    }

    /// <summary>
    /// Numbering of the panels from 1... up to 8
    /// </summary>
    private IEnumerator WaitAndNumeratePanel()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject[] allPanels = GameObject.FindGameObjectsWithTag("Cell");

        for (int a = 0; a < allPanels.Length; a++)
        {
            allPanels[a].GetComponentInChildren<CountCells>().SetPanelNumber(a + 1);
        }
        ReorganizeStoryboard();
        yield return null;
    }

    /// <summary>
    /// For the case loading/new Storyboard the old Props have to be destroyed
    /// </summary> 
    public void DestroyPropsInSpawnContainer()
    {      
        GameObject[] childrenOfPropsSpawnContainer = GameObject.FindGameObjectsWithTag("PropsContainer");
        for (int a = 0; a < childrenOfPropsSpawnContainer.Length; a++)
        {
            if (childrenOfPropsSpawnContainer[a].transform.name != "CloneDummy(Clone)")
            {
                Destroy(childrenOfPropsSpawnContainer[a]);
            }
        }
    }

    /// <summary>
    /// In case a panel has to be deleted, or the whole storyboard is new or loaded, all old props must be destroyed
    /// </summary> 
    public void DestroyAllPanels()
    {
        GameObject[] panelContainer = GameObject.FindGameObjectsWithTag("Cell");
        for (int b = 0; b < panelContainer.Length; b++)
        {
            Destroy(panelContainer[b]);
        }
        Resources.UnloadUnusedAssets();
    }
}
