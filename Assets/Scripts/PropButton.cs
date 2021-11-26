using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// Superclass for buttons controlling props placed in scene.
/// </summary>
public abstract class PropButton : MonoBehaviour, IInputClickHandler {

    /* 
     * Default and first color used for button background
     */
    protected Color initialColor = Color.black;

    /// <summary>
    /// Start setup
    /// </summary>
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = initialColor;
    }

    protected abstract void PropButtonStart ();  

    /* Used for handling the button click
     * 
     */
    public abstract void OnInputClicked (InputClickedEventData eventData);
}
