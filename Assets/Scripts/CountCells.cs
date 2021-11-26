using UnityEngine;

/// <summary>
/// Stores the actual position of each Panel on the storyboard
/// Returns the panel number
/// </summary>
public class CountCells : MonoBehaviour {

    #region Init

    public int panelNumber;
    public GameObject transitionDropDown;
    public GameObject deleteButton;
    private GameObject [] allPanels;
    public GameObject panel;
    public GameObject propsContainer;

    #endregion

    /// <summary>
    /// Set the proper count value on App-start 
    /// </summary>
    void Start () {
        RecalculatePanelNumber();
    }

    /// <summary>
    /// Disables the last dropdown field.
    /// Changes the props containers name if a panel was deleted.
    /// </summary>
    void Update () {
        if (gameObject.activeInHierarchy)
        {
            if (panelNumber == 8)
            {
                transitionDropDown.SetActive(false);
            }
            else
            {
                transitionDropDown.SetActive(true);
            }
            if (panelNumber == 1 || panelNumber == 2)
            {
                DisableDeleteButton();
            }
            else
            {
                EnableDeleteButton();
            }
        }

        /*
         * Rename Container if something changed on the GUI
         */

        SetPropsContainerName(panelNumber);
	}

    /// <summary>
    /// Changes the value of the Panel number
    /// </summary>
    public void IncreaseCounter()
    {
        panelNumber++;
        SetPanelNumber(panelNumber);
    }

    /// <summary>
    /// Returns the actual Panel number 
    /// </summary>
    public int GetPanelNumber()
    {
        return panelNumber;
    }

    /// <summary>
    /// Changes the value if needed
    /// </summary>
    public void SetPanelNumber(int newPanelNr)
    {
        panelNumber = newPanelNr;
    }

    /// <summary>
    /// Deactivation of a Panel
    /// </summary>
    public void DestroyCell()
    {
        panel.GetComponent<DestroyCell>().DeactivateSelf();
    }

    /// <summary>
    /// The first two panels should not be deleted, So their delete buttons are always in disabled state.
    /// </summary>
    private void DisableDeleteButton()
    {     
        deleteButton.SetActive(false);
    }

    /// <summary>
    /// Change state to active
    /// </summary>
    private void EnableDeleteButton()
    {
        deleteButton.SetActive(true);
    }

    /// <summary>
    /// When a panel has been deleted, the panel numbers must be recalculated.
    /// </summary>
    public void RecalculatePanelNumber()
    {
        allPanels = GameObject.FindGameObjectsWithTag("Cell");
        for (int a = 0; a < allPanels.Length; a++)
        {
            allPanels[a].GetComponentInChildren<CountCells>().SetPanelNumber(a + 1);                      
        }       
    }

    /// <summary>
    /// To find the proper container for the instantiation of the props, the gameobject has to be renamed and added the cell number to its name.
    /// </summary>
    /// <param name="a">A unique number for the props container.</param>
    public void SetPropsContainerName(int a)
    {
        propsContainer.name = "Container_" + a.ToString();
    }
}
