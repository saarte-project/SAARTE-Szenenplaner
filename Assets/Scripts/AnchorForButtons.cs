using UnityEngine;

/// <summary>
/// Anchor position for buttons.
/// Called by the "AnchorButtons" script.
/// </summary>
public class AnchorForButtons : MonoBehaviour
{
    #region Init

    public Vector3 anchorPosition;
    private GameObject buttons;

    #endregion

    /// <summary>
    /// Anchor position for buttons. The position is hardcoded in the Unity Inspector.
    /// Called by the "AnchorButtons" script.
    /// </summary>
    public Vector3 GetAnchorPosition()
    {
        return anchorPosition;
    }
}
