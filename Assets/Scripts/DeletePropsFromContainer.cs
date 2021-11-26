using UnityEngine;

/// <summary>
/// In case the user deletes a panel, the matching Instance_Container for the props must also be destroyed. 
/// See Class 'AddPropsToButton' for more details about the creation of this instances.
/// </summary>
public class DeletePropsFromContainer : MonoBehaviour {

    #region Init

    public GameObject panelContent;
    public GameObject propsContainer;

    #endregion

    /// <summary>
    ///  Destroy the propContainer and the dependencies (the Props)
    /// </summary>
    public void DestroyPropsInContainer()
    {      
        int panelNumber = panelContent.GetComponent<CountCells>().GetPanelNumber();
        propsContainer.GetComponent<AddPropsToContainer>().DestroyPropSelf(panelNumber);
    }
}
