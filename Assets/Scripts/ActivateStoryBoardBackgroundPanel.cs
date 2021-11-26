using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

/// <summary>
/// Activates a frame around a panel after the user had clicked on a frame component(e.g. a button/dropdown...) 
/// It also deactivates all other frames in the scene (if one is active).
/// Furthermore it shrinks all Props to make them invisible for the user. Only the Props of the active panel will be resized
/// </summary>
public class ActivateStoryBoardBackgroundPanel : MonoBehaviour
{
    #region Init

    private GameObject[] allBackgrounds;
    private GameObject[] allPanelMenus;
    public GameObject panelContent;
    private bool syncButtons;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        gameObject.AddComponent<DestroyAllProps>();

        /*
         * The "syncButtons" variable was created to avoid race conditions. 
         */

        syncButtons = false;
    }

    /// <summary>
    /// After clicking on a panel, a frame around the panel appears. All other active frames disappear.
    /// The related props are visible again, all other Props disappear.
    /// If a panel menu on another panel was active, it will also get closed.
    /// </summary>
    public void ActivateBackground()
    {
        allBackgrounds = GameObject.FindGameObjectsWithTag("PanelBackground");
        allPanelMenus = GameObject.FindGameObjectsWithTag("PanelMenu");

        /*
         * Deactivate all Frames exept the one that should be active
         */

        if (gameObject.GetComponent<Image>().enabled != true)
        {
            for (int a = 0; a < allBackgrounds.Length; a++)
            {
                Image tmpImage = allBackgrounds[a].GetComponent<Image>();
                Assert.IsNotNull(tmpImage, "Object does not have expected Image component.");
                tmpImage.enabled = false;
            }
            gameObject.SetActive(true);
            gameObject.GetComponent<Image>().enabled = true;
            // Shrink all Props to make them invisible for the User after changing the Panel
            if (gameObject.GetComponent<DestroyAllProps>())
            {
                gameObject.GetComponent<DestroyAllProps>().ToggleAllPropsInScene(new GameObject(), false);
            }

            /*
             * Resize all Props of activated Panel to make them visible again
             */

            int panelNumber = panelContent.GetComponent<CountCells>().GetPanelNumber();
            GameObject tempPropContainer = GameObject.Find("_Instance.Container_" + panelNumber) as GameObject;
            int propsContainerSize = tempPropContainer.transform.childCount;
            if (gameObject.GetComponent<DestroyAllProps>())
            {
                gameObject.GetComponent<DestroyAllProps>().ToggleAllPropsInScene(tempPropContainer, true);
            }           
        }
        for (int b = 0; b < allPanelMenus.Length; b++)
        {
            allPanelMenus[b].SetActive(false);
        }

        syncButtons = true;
    }

    /// <summary>
    /// Returns the actual state of the "syncButtons"
    /// </summary>
    public bool GetSyncButtons()
    {
        return syncButtons;
    }
}
