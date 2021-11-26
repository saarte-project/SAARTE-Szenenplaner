using UnityEngine;

/// <summary>
/// The scrollView within the PanelMenu does not expand properly when the buttons for the saved files reach the size of the viewport. 
/// This causes a false scrollbar size and thus with unreachable buttons.
/// The script expands the Transform in the Update method, so it always has optimized vertical heigt and the scrollbar is working properly.
/// </summary>
public class ExpandRectTransform : MonoBehaviour {

    #region Init

    private int numberOfFiles;
    private RectTransform rectHeight;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    void Start ()
    {
        numberOfFiles = transform.childCount;
        rectHeight = GetComponent(typeof(RectTransform)) as RectTransform;      
    }

    /// <summary>
    /// Calculates n * 50f and sets the height of the RectTransform
    /// </summary>
    void Update ()
    {
        numberOfFiles = transform.childCount;
        rectHeight.sizeDelta = new Vector2(rectHeight.sizeDelta.x, numberOfFiles * 50f);
	}
}
