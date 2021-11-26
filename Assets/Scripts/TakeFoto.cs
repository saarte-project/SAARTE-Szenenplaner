using UnityEngine.XR.WSA.WebCam;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using Saarte;

/// <summary>
/// This class implements the photo capture and sets the taken picture as a texture for the panel
/// </summary>
public class TakeFoto : MonoBehaviour
{
    #region Init

    PhotoCapture photoCaptureObject = null;
    private GameObject cam;
    private GameObject panel;
    public TextDisplayForTime infoText;
    private GameObject pictureFrame;
    public GameObject cellBackground;
    [SerializeField]
    [Tooltip("The connected Cursor has to disappear before the photo gets captured.")]
    protected GameObject cursor;
    private bool isCurrentPhotoScript;
    public GameObject storyBoardPanel;
    Texture2D targetTexture;
    public Texture2D dummyTexture;
    public float newX;
    public float newY;
    public float newWidth;
    public float newHeight;
    RawImage rawImage = null;
    private bool timeOut;
    float setTimer;
    public AudioClip clickSound;
    public AudioSource musicSource;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        timeOut = false;
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        pictureFrame = cam.GetComponent<ActivateFrame>().getFrame();
        panel = transform.parent.gameObject;
        rawImage = panel.GetComponentInChildren<RawImage>();
        isCurrentPhotoScript = false;
        cursor = GameObject.FindGameObjectWithTag("Cursor");
        setTimer = 0;
        musicSource.clip = clickSound;
    }

    /// <summary>
    /// Activates a green frame which represents a viewport and starts a timeout after the click to avoid an possible app-crash when callback methods do not work properly
    /// Function is connected to a button on the Staryboard
    /// </summary>
    public void OnButtonClicked()
    {
        isCurrentPhotoScript = true;
        cam.GetComponent<ActivateFrame>().SetTakePhotoScript(gameObject);
        pictureFrame.SetActive(true);
        timeOut = true;
    }

    /// <summary>
    /// Responsible to handle the timeout.
    /// Resets all changes made by this script if callback methods do not work proper
    /// </summary>
    private void Update()
    {      
        if (timeOut)
        {         
            setTimer += Time.deltaTime;
            if(setTimer > 15)
            {
                cursor.SetActive(true);
                cam.GetComponent<ActivateFrame>().activateText();
                cam.GetComponent<ActivateFrame>().deactivateFrame();
                cam.GetComponent<ActivateFrame>().UnsetTakePhotoScript();
                StopAllCoroutines();
                timeOut = false;
                setTimer = 0;
                GetComponent<HoloMessage>().setDebugMessage("Zeitüberschreitung!", 1f, new Color(255, 0, 0));
            }
        }
    }

    /// <summary>
    /// If an air tap is performed, the photo capture process starts.
    /// The button logic for calling this function is located on a Gameobject in front of the camera, to fetch the airtap.
    /// </summary>
    public void OnInputClickedInActivateFrame()
    {
        if (isCurrentPhotoScript)
        {
            isCurrentPhotoScript = false;
            cursor.SetActive(false);
            cam.GetComponent<ActivateFrame>().deactivateText();
            OnExecuteInputClicked();
        }
    }

    /// <summary>
    /// Input click handler for the use with the Hololens.
    /// Starts the photo creation process
    /// </summary>
    public void OnExecuteInputClicked()
    {      
        try
        {
            PhotoTake();
        }
        catch (Exception e)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnExecuteInputClicked: " + e, 20f, new Color(255, 0, 0));
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// Prepare setup for the photo creation.
    /// Depending, whether is is triggered in the editor or by the Hololens, a real photo (Hololens)or dummy image (Editor) will be the result
    /// </summary>
    void PhotoTake()
    {         
#if UNITY_EDITOR
        try
        {
            storyBoardPanel.GetComponent<JPGtoByte>().SetByteArray(dummyTexture);
            rawImage.uvRect = new Rect(newX, newY, newWidth, newHeight);
            rawImage.texture = dummyTexture;
            cursor.SetActive(true);
            cam.GetComponent<ActivateFrame>().activateText();
            cam.GetComponent<ActivateFrame>().deactivateFrame();
            cam.GetComponent<ActivateFrame>().UnsetTakePhotoScript();
            timeOut = false;
            setTimer = 0;
        }
        catch(Exception exe)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + exe, 20f, new Color(255, 0, 0));
            StopAllCoroutines();
        }
#endif

#if !UNITY_EDITOR
        //set true for showing the holograms on the picture
        Debug.Log("No Unity Editor");
        try {
            PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
        }
        catch(Exception photoTake)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + photoTake, 20f, new Color(255, 0, 0));

        }
#endif
    }

    /// <summary>
    /// A step in the callback cascade
    /// </summary>
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        try
        {
            photoCaptureObject = captureObject;
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 1.0f;
            c.cameraResolutionWidth = cameraResolution.width;
            c.cameraResolutionHeight = cameraResolution.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }
        catch(Exception OnPhoto)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + OnPhoto, 20f, new Color(255, 0, 0));
        }
    }

    /// <summary>
    /// First step in the callback cascade
    /// </summary>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        try
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }
        catch (Exception OnStopped)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + OnStopped, 20f, new Color(255, 0, 0));
        }

    }

    /// <summary>
    /// A step in the callback cascade
    /// </summary>
    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {     
        try
        {
            if (result.success)
            {
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
            else
            {
                GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoModeStarted: ", 20f, new Color(255, 0, 0));
            }
        }
        catch (Exception OnPhotoSTarted)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + OnPhotoSTarted, 20f, new Color(255, 0, 0));
        }
    }

    /// <summary>
    /// Last step in the callback cascade.
    /// The image is processed and in storage, now it can be saved to file system.
    /// the image will be cropped and copied as a RawImage to the Panel which called the "TakePhoto" script.
    /// </summary>
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        /*
         * A click sound as a simulation of a real camera
         */

        PlayCLickSound();

        try
        {
            if (result.success)
            {
                /* 
                 * Taking only part of the image as texture.   
                 */

                rawImage.uvRect = new Rect(newX, newY, newWidth, newHeight);

                /*
                 * Create our Texture2D for use and set the correct resolution
                 */

                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

                /* 
                 * Copy the raw image data into our target texture
                 */

                photoCaptureFrame.UploadImageDataToTexture(targetTexture);
                rawImage.texture = targetTexture;
            }

            /*
             * Clean up and reset all changes made in the UI by ths script
             */

            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            cursor.SetActive(true);
            cam.GetComponent<ActivateFrame>().activateText();
            cam.GetComponent<ActivateFrame>().deactivateFrame();
            cam.GetComponent<ActivateFrame>().UnsetTakePhotoScript();
            timeOut = false;
            setTimer = 0;
        }
        catch (Exception OnCaptured)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while OnPhotoCaptureCreated: " + OnCaptured, 20f, new Color(255, 0, 0));
        }


        /*
         * Create a byte Array and store it to the Instance of the Panel Prop. This is needed for faster savings.
         */

        try
        {
            storyBoardPanel.GetComponent<JPGtoByte>().SetByteArray(targetTexture);
        }
        catch (Exception ex)
        {
            GetComponent<HoloMessage>().setDebugMessage("Error while creating a Byte Array: " + ex, 20f, new Color(255, 0, 0));
        }
    }

    /// <summary>
    /// Plays a custom click sound 
    /// </summary>
    private void PlayCLickSound()
    {
        musicSource.Play();
    }
}
