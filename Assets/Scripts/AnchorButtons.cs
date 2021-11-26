using UnityEngine;

/// <summary>
/// If Props change their size, the attached Buttons can unfortunaly also change their position relative to the Prop. 
/// To avoid this, the position of the Buttons is anchored to the Props, by using this logic in the Update method.
/// </summary>
public class AnchorButtons : MonoBehaviour {

    /// <summary>
    /// Sticks buttons
    /// </summary>
    private void Update()
    {
        transform.localPosition = GetComponentInParent<AnchorForButtons>().GetAnchorPosition();
    }
}
