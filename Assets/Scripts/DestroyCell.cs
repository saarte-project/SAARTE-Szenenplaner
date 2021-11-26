using UnityEngine;

public class DestroyCell : MonoBehaviour {

    /// <summary>
    /// Destroys a panel after the deletion button was pressed.
    /// It also clones the the deleted panel to 'Deleted Panels" and deactivates it afterwards.
    /// From here, the panel can be restored in order to use it again later.
    /// This functionality is not yet implemented and only serves future purposes
    /// </summary>
    public void DeactivateSelf()
    {
        GameObject panelListCanvas = GameObject.FindGameObjectWithTag("PanelListCanvas");
        GameObject deletedPanels = GameObject.FindGameObjectWithTag("Deleted");     
        GameObject tempPanel = Instantiate(gameObject, deletedPanels.transform);
        tempPanel.SetActive(false);
        panelListCanvas.GetComponent<PanelManager>().OnRemoveButtonPressed(gameObject);      
    }
}
