using UnityEngine;

/// <summary>
/// Collects all props in the scene and destroys them. 
/// Also collects props with are in an undefined state. This can happen, when the callback functions where not triggered
/// after placing the prop in the scene. This is an issue and further investigation is recommended.
/// </summary>
public class DestroyAllProps : MonoBehaviour
{
    #region Init

    public GameObject panelContent;
    public void DestroyAllPropsInScene()
    {
        if (GameObject.FindGameObjectsWithTag(StaticVariables.objectTag) != null)
        {
            foreach (GameObject _gameObject in GameObject.FindGameObjectsWithTag(StaticVariables.objectTag))
            {
                Destroy(_gameObject);
            }
        }
        if (GameObject.FindGameObjectsWithTag(StaticVariables.objectTagUndefinedState) != null)
            foreach (GameObject _gameObject in GameObject.FindGameObjectsWithTag(StaticVariables.objectTagUndefinedState))
            {
                Destroy(_gameObject);
            }
    }

    #endregion

    /// <summary>
    /// Destroy the props for a panel, depending from which tab the user clicks
    /// </summary>
    public void DestroyProps()
    {
        string name = "";
        int panelnumber;
        if(transform.parent.name =="PanelTech")
        {
            name = "_PanelTech";
        }
        if (transform.parent.name == "PanelRequisite")
        {
            name = "_PanelRequisite";
        }
        panelnumber = panelContent.GetComponent<CountCells>().GetPanelNumber();
        GameObject propsContainer = GameObject.Find("_Instance.Container_" + panelnumber);
        int childCounter = propsContainer.transform.childCount;
        for (int a = 0; a < childCounter; a++)
        {          
            if(propsContainer.transform.GetChild(a).transform.name.Contains(name))
            {
                Destroy(propsContainer.transform.GetChild(a).gameObject);
            }
        }
        Resources.UnloadUnusedAssets();
    }


    /// <summary>
    /// The moment the user selects a panel, the size of all containers will be deactivated. The user cannot see the props anymore.
    /// This also happens after the loading process.
    /// </summary>
    public void ActivatePropForPanel(GameObject propContainer)
    {
        propContainer.SetActive(true);
    }

    /// <summary>
    /// After the user activated a panel, the proper container will be activated.
    /// </summary>
    public void DeactivatePropForPanel(GameObject propContainer)
    {
        propContainer.SetActive(false);
    }

    /// <summary>
    /// After the user activated a panel, the Props will be visible, all other invisible.
    /// </summary>
    public void ToggleAllPropsInScene(GameObject container, bool toggle)
    {
        if (toggle)
        {
            GameObject propContainer = container;
            if (propContainer.transform.childCount > 0)
            {
                for (int x = 0; x < propContainer.transform.childCount; x++)
                {
                    GameObject tempChild = propContainer.transform.GetChild(x).gameObject;
                    tempChild.SetActive(toggle);
                }
            }
        }
        else
        {
            GameObject[] allPropContainer;
            allPropContainer = GameObject.FindGameObjectsWithTag("PropsContainer");
            for (int a = 0; a < allPropContainer.Length; a++)
            {
                GameObject tempContainer = allPropContainer[a];
                for (int b = 0; b < tempContainer.transform.childCount; b++)
                {
                    GameObject tempChild = tempContainer.transform.GetChild(b).gameObject;
                    tempChild.SetActive(toggle);
                }
            }
        }
    }
}
