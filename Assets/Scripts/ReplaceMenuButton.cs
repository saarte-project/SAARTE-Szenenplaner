using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// Calls the method 'tapToPlaceMenue.ButtonStartPlacement()', which enables the functionality to move a gameobject in the virtual world.
/// After a mouse click the gameobject is anchored in the world again
/// </summary>
public class ReplaceMenuButton : MonoBehaviour, IInputClickHandler
{
    #region Init

    public TapToPlaceMenu tapToPlaceMenue;

    #endregion

    /// <summary>
    /// Implements a button click logic ready to use with the Hololens gestures
    /// </summary>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        tapToPlaceMenue.ButtonStartPlacement();
        eventData.Use();
    }
}
