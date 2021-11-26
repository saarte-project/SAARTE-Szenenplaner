using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using System.Collections;

/// <summary>
/// When the user instantiates a prop in the scene, it may be that there is already an active prop.
/// In that case, the old prop will be destroyed and the new one is added in front of the user.
/// </summary>
public class AddPropsToButton : MonoBehaviour {

    #region Init

    public GameObject propName;
    public GameObject panelMenu;
    public GameObject panelContent;
    public GameObject propsContainer;
    private GameObject actualPropInHand;

    #endregion

    /// <summary>
    /// The actual Prop, actual in movement mode
    /// </summary>
    private void Start()
    {
        actualPropInHand = panelMenu.GetComponent<ToggleStoryBoardSubmenuProps>().GetActualPropInHand();
    }

    /// <summary>
    /// Destroy all Props in wrong mode or in users 'hand'. 
    /// Create a new Prop, depending which button was pressed
    /// </summary>
    public void spawnObject()
    {
        DestroyPossibleWrongSetProps();
        StartCoroutine(DestroyPossiblePropInHand());
        StartCoroutine(createNewProp(transform.parent.transform.name));
    }

    /// <summary>
    /// Destroys a Prop, if one is in 'hand' while tzhe user clicks a spawn button
    /// </summary>
    private IEnumerator DestroyPossiblePropInHand()
    {     
        actualPropInHand = panelMenu.GetComponent<ToggleStoryBoardSubmenuProps>().GetActualPropInHand();
        if (actualPropInHand)
        {
            Destroy(panelMenu.GetComponent<ToggleStoryBoardSubmenuProps>().GetActualPropInHand());
        }
        yield return new WaitForSeconds(0.3f);
        yield return true;
    }

    /// <summary>
    /// Instantiates a prop in front of the user.
    /// </summary>
    private  IEnumerator createNewProp(string panelName)
    {
        bool isPropLoadedFromFile = false;
        PropsFactory createPreviewObject = GameObject.FindObjectOfType(typeof(PropsFactory)) as PropsFactory;
        int actualCellNumber = -1;
        actualCellNumber = panelContent.GetComponent<CountCells>().GetPanelNumber(); 
        panelMenu.GetComponent<ToggleStoryBoardSubmenuProps>().SetActualPropInHand(createPreviewObject.SpawnObject(propName,isPropLoadedFromFile,actualCellNumber,"_"+panelName));
        yield return true;
    }

    /// <summary>
    /// The user can move the props in the scene. This can cause issues in case that the prop is set into a wall or into other virtual objects.
    /// To avoid this, the 'isPropInHand' variable will be checked. In case the result is 'true', the prop will be deleted from the panel.
    /// </summary>
    private void DestroyPossibleWrongSetProps()
    {      
        GameObject[] allProps = GameObject.FindGameObjectsWithTag("AVP_Object");
        for (int a = 0; a < allProps.Length; a++)
        {
            if (allProps[a].GetComponent<TapToPlaceCustom>().propIsInHand || allProps[a].GetComponent<TapToPlaceCustom>().IsBeingPlaced)
            {
                Debug.Log("Destroyed wrong set prop");
                Destroy(allProps[a]);
            }
        }
        GameObject[] allPropsMoving = GameObject.FindGameObjectsWithTag("AVP_Object_Moving");
        for (int a = 0; a < allPropsMoving.Length; a++)
        {
            if (allPropsMoving[a].GetComponent<TapToPlaceCustom>().propIsInHand || allPropsMoving[a].GetComponent<TapToPlaceCustom>().IsBeingPlaced)
            {
                Destroy(allPropsMoving[a]);
            }
        }
    }
}
