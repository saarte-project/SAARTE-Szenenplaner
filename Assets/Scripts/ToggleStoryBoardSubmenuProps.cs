using UnityEngine;

/// <summary>
/// Toggles between two panels (Tech/Requisite)
/// </summary>
public class ToggleStoryBoardSubmenuProps : MonoBehaviour {

    #region Init

    public GameObject subMenu1;
    public GameObject subMenu2;
    public GameObject actualPropInHand;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        subMenu1.SetActive(true);
        subMenu1.SetActive(false);
    }

    /// <summary>
    /// Enables the requisite Panel als disables the tech Panel
    /// </summary>
    public void enableRequisite()
    {
        subMenu1.SetActive(true);
        subMenu2.SetActive(false);
    }

    /// <summary>
    /// Enables the tech Panel als disables the requisite Panel
    /// </summary>
    public void enableTech()
    {
        subMenu1.SetActive(false);
        subMenu2.SetActive(true);
    }

    /// <summary>
    /// Sets the actual Prop, which is manipulated by the user
    /// </summary>
    public void SetActualPropInHand(GameObject actualProp)
    {
        actualPropInHand = actualProp;
    }

    /// <summary>
    /// Sets the actual Prop, which is manipulated by the user
    /// </summary>
    public GameObject GetActualPropInHand()
    {
        return actualPropInHand;
    }
}
