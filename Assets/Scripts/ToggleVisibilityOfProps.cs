using UnityEngine;

/// <summary>
/// Changes the size of the props to make them visible or invisible
/// The button for this functionality is located at the 'Panel Menu' GameObject.
/// </summary>
public class ToggleVisibilityOfProps : MonoBehaviour
{
    #region init

    private GameObject propsContainer;
    public GameObject panelContent;
    private int panelNumber;
    GameObject containerInstance;

    #endregion

    /// <summary>
    /// Find the Spawncontainer for the Instances
    /// </summary>
    private void Start()
    {
        propsContainer = GameObject.FindGameObjectWithTag("PropsSpawnContainer");
    }

    /// <summary>
    /// Make the Props visible or invisible
    /// </summary>
    public void OnButtonPressed()
    {
        panelNumber = panelContent.GetComponent<CountCells>().GetPanelNumber();
        containerInstance = propsContainer.transform.Find("_Instance.Container_" + panelNumber).gameObject;
        int containerSize = containerInstance.transform.childCount;
        string panelName = transform.parent.name;
        if (containerSize > 0)
        {
            GameObject tempGameObject;
            for (int x = 0; x < containerSize; x++)
            {
                tempGameObject = containerInstance.transform.GetChild(x).gameObject;
                string childName = tempGameObject.transform.name;
                string childFatherName = "";
                if (childName.Contains("PanelTech"))
                {
                    childFatherName = "PanelTech";
                }
                if (childName.Contains("PanelRequisite"))
                {
                    childFatherName = "PanelRequisite";
                }

                if (transform.parent.name == childFatherName && !tempGameObject.activeInHierarchy)
                {
                    gameObject.GetComponent<DestroyAllProps>().ActivatePropForPanel(tempGameObject);
                }
                else if (transform.parent.name == childFatherName && tempGameObject.activeInHierarchy)
                {
                    gameObject.GetComponent<DestroyAllProps>().DeactivatePropForPanel(tempGameObject);
                }
            }
        }
    }
}
