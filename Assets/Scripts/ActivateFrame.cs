using UnityEngine;
using HoloToolkit.Unity.InputModule;

/// <summary>
/// This script is called from the 'TakePhoto' script. It activates/deactivates a green frame while taking the photo.
/// It also disables a text component, so it will not be part of the picture.
/// </summary>
public class ActivateFrame : MonoBehaviour, IInputClickHandler
{
    #region

    public GameObject frame;
    public GameObject text;
    public GameObject takePhotoScript;

    #endregion

    /// <summary>
    /// Click event handler. Activates a green frame in the picture to visualize the viewport
    /// </summary>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(takePhotoScript != null)
        {
            takePhotoScript.GetComponent<TakeFoto>().OnInputClickedInActivateFrame();
        }
    }
        public void activateFrame()
    {
        frame.SetActive(true);      
    }

    /// <summary>
    /// deactivates the frame after the picture is processed. Called from TakePhoto script
    /// </summary>
    public void deactivateFrame()
    {
        frame.SetActive(false);
    }

    /// <summary>
    /// Returns the Frame to the TakePhoto script.
    /// </summary>
    public GameObject getFrame()
    {
        return frame;
    }

    /// <summary>
    /// Deactivates a text in the viewport
    /// </summary>
    public void deactivateText()
    {
        text.SetActive(false);
    }

    /// <summary>
    /// Activates a text in the viewport
    /// </summary>
    public void activateText()
    {
        text.SetActive(true);
    }

    /// <summary>
    /// A reference to the existing TakePhoto script
    /// </summary>
    public void SetTakePhotoScript(GameObject takePhoto)
    {
        takePhotoScript = takePhoto;
    }

    /// <summary>
    /// A reference to the existing TakePhoto script will be null
    /// </summary>
    public void UnsetTakePhotoScript()
    {
        takePhotoScript = null;
    }
}
