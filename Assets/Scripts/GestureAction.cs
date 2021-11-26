using HoloToolkit.Unity.InputModule;
using UnityEngine;
using System.Collections;
using HoloToolkit.Unity.SpatialMapping;

/// <summary>
/// The class GestureAction implements the two mode (Rotate Mode and Move Mode).
/// </summary>
public class GestureAction : MonoBehaviour, IManipulationHandler
{
    #region Init

    /// <summary>
    /// reference to the rigidbody of the object.
    /// </summary>
    private Rigidbody rb;
    /// <summary>
    /// Is the transform of the head from the object.
    /// </summary>
    private Transform transformChildObject;

    /// <summary> Usualy describes the position of the object at the start of the interaction and will be set at the start of an interaction </summary>
    private Vector3 manipulationOriginalPosition = Vector3.zero;
    /// <summary> Usualy describes the rotation of the object at the start of the interaction and will be set at the start of an interaction</summary>
    private Quaternion navigationOriginalRotation;
    /// <summary> Usualy used to describe the rotation of the head of the object at the start of the interaction and will be set at the start of an interaction</summary>
    private Quaternion navigationChildOriginalRotation;
    /// <summary> Describes if the mode for moving the object along the xyz axes is enabled (default mode) </summary>
    /// ???gets set?
    /// This is also the default mode which gets set active if no other mode is active.
    private bool isMoveEnabled = false;
    /// <summary>
    /// Describes if the rotationmode is enabled for the object.
    /// </summary>
    private bool isRotationEnabled = false;
    /// <summary>
    /// Describes if the rotation for the head of the object is active.
    /// </summary>
    private bool isRotationForChildEnabled = false;

    #endregion

    /// <summary>
    /// Enables the move option on startup
    /// </summary>
    public void Start()
    {
        EnableMove();
    }

    /// <summary>
    /// Start the move operation or rotation operation at the beginning of a manipulation.
    /// </summary>
    /// Takes the initial rotation or position, depending on the mode which is selected for later use.
    /// <param name="eventData">Is not used</param>
    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        StartCoroutine(RemoveGestureActionScript());
        InputManager.Instance.ClearModalInputStack();
        if (isRotationEnabled || isRotationForChildEnabled)
        {
            transformChildObject = gameObject.transform.Find("Head");
            navigationOriginalRotation = transform.rotation;
            if (transformChildObject)
            {
                navigationChildOriginalRotation = transformChildObject.rotation;
            }
        }
        if (isMoveEnabled)
        {
            
            GetComponent<TapToPlaceCustom>().propIsInHand = true;
            manipulationOriginalPosition = transform.position;
        }
        InputManager.Instance.PushModalInputHandler(gameObject);
    }

    /// <summary>
    /// The object is moved or rotated according to the gesture performed and according to which mode is enabled.
    /// </summary>
    /// This function will be called every update (don't know when and how often),
    /// so the calculation of the rotation/ movement is done with the original rotation/ position of the object to prevent a continuous rotation or movement
    /// and to let the object follow or copy the movement of the users hand.
    /// In the rotation part of the function the coordinates of the hand movement are transformed in the local space of the Hololense.
    /// This is necessary so that the rotation is independent from the direction the user interacts with the object.
    /// 
    /// Relevant attributes: 
    /// 
    /// Quaternion navigationOriginalRotation          || is the rotation of the object at the start of the interaction
    /// 
    /// Quaternion navigationChildOriginalRotation     || is the rotation of the head from the object at the start of the interaction
    /// 
    /// Vector3 manipulationOriginalPosition           || is the position of the object at the start of the interaction
    /// 
    /// float StaticVariables.rotationSesitivity  || describes how sensitve the rotation will be 1000 is a relativ good value
    /// </summary>
    /// <param name="eventData">Describes an input event that involves content manipulation (look in the documentation from Holotoolkit)</param>
    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (isRotationEnabled)
        {
            Transform HoloTransform = GameObject.Find("HoloLensCamera").transform;
            Vector3 cumulativeDeltaTransformed = HoloTransform.InverseTransformDirection(eventData.CumulativeDelta);
            transform.rotation = navigationOriginalRotation * Quaternion.Euler(Vector3.up * cumulativeDeltaTransformed.x * StaticVariables.rotationSensitivity * -1);
        }
        else if (isRotationForChildEnabled)
        {
            Transform HoloTransform = GameObject.Find("HoloLensCamera").transform;
            Vector3 cumulativeDeltaTransformed = HoloTransform.InverseTransformDirection(eventData.CumulativeDelta);
            transformChildObject.rotation = navigationChildOriginalRotation * Quaternion.Euler(Vector3.up * cumulativeDeltaTransformed.x * StaticVariables.rotationSensitivity * -1);
        }

        if (isMoveEnabled)
        {
            /*
             * CumulativeDelta is the amount of the manipulation that has occured. Usually in the form of delta position of a hand.
             */

            transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
        }
    }

    /// <summary>
    /// The move or rotation operation is completed.
    /// </summary>
    /// <param name="eventData">Is not used</param>
    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        GetComponent<Transform>().rotation = new Quaternion(GetComponent<Transform>().rotation.x, GetComponent<Transform>().rotation.y, GetComponent<Transform>().rotation.z, GetComponent<Transform>().rotation.w);
        StartCoroutine(AddGestureActionScript());
        if (isRotationEnabled)
        {
        }
        if (isMoveEnabled)
        {
            GetComponent<Rigidbody>().freezeRotation = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            if (gameObject.name != "Head")
            {
                transform.tag = "AVP_Object_Moving";
            }
            
            StartCoroutine(SetRigidbodyToKinematic());
            GetComponent<TapToPlaceCustom>().propIsInHand = false;
        }

        InputManager.Instance.PopModalInputHandler();
    }

    /// <summary>
    /// The move gesture is canceled.
    /// genauer beschreiben? anschauen?
    /// </summary>
    /// <param name="eventData">Is not used</param>
    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        StartCoroutine(AddGestureActionScript());
        if (isRotationEnabled)
        {
        }

        if (isMoveEnabled)
        {
            GetComponent<Rigidbody>().freezeRotation = true;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            if (gameObject.name != "Head")
            {
                transform.tag = "AVP_Object_Moving";
            }
            
            StartCoroutine(SetRigidbodyToKinematic());
            GetComponent<TapToPlaceCustom>().propIsInHand = false;
        }
        InputManager.Instance.PopModalInputHandler();
    }

    /// <summary>
    /// The Props in the scene should not be physically affected by other moving props when they are placed
    /// </summary>
    private IEnumerator SetRigidbodyToKinematic()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Rigidbody>().freezeRotation = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<TapToPlaceCustom>().propIsInHand = false;
        if (gameObject.name != "Head")
        {
            gameObject.tag = "AVP_Object";
        }
        yield return null;
    }

    /// <summary>
    /// The "GestureAction" scripts are added to the Props in the scene, to implement move and rotation logic. 
    /// Unfortunally, the click event can be triggereed in multiple Props at the same time, which results in movement/rotation of props which should not change their state.
    /// This happens, when a gesture of the users hand is performed and the raycast hits multiple Gameobjects.
    /// To avoid this, all "GestureAction" scripts on all other Props in the scene will be removed for the time the manipulation will take.
    /// The 'Flag' to distinguish moving or placed Props if the tag of them. 
    /// If a Prop is activated for moving, the Tag name changes from "AVP_Object" to "AVP_Object_Moving"
    /// </summary>
    private IEnumerator RemoveGestureActionScript()
    {
        if (gameObject.name != "Head")
        {
            gameObject.tag = "AVP_Object_Moving";
        }
        GameObject[] allAVPObjects = GameObject.FindGameObjectsWithTag("AVP_Object");
        for (int remove = 0; remove < allAVPObjects.Length; remove++)
        {
            GestureAction tempScript = allAVPObjects[remove].GetComponent<GestureAction>();
            Destroy(tempScript);
        }
        yield return true;
    }

    /// <summary>
    /// The moment the manipulation ends, all Props will get back their GestureAction script
    /// </summary>
    private IEnumerator AddGestureActionScript()
    {
        GameObject[] allAVPObjects = GameObject.FindGameObjectsWithTag("AVP_Object");
        for (int add = 0; add < allAVPObjects.Length; add++)
        {
            if (allAVPObjects[add].gameObject.tag != "AVP_Object_Moving")
            {
                try
                {
                    if (allAVPObjects[add].GetComponent<GestureAction>())
                    {
                        // Do nothing now
                    }
                    else
                    {
                        allAVPObjects[add].AddComponent<GestureAction>();
                    }
                }
                catch (System.Exception exept)
                {
                    Debug.Log("Double" + exept);
                }
            }
        }
        if (gameObject.name != "Head")
        {
            gameObject.tag = "AVP_Object";
        }
        yield return true;
    }

    /// <summary>
    /// Enables the rotation of the object by setting the variables accordingly
    /// </summary>
    ///Gets called by the buttons beneath the object
    public void EnableRotation()
    {
        isRotationEnabled = true;
        isRotationForChildEnabled = false;
        isMoveEnabled = false;
    }

    /// <summary>
    /// Enables the rotation of the head from the object by setting the variables accordingly
    /// </summary>
    public void EnableChildRotation()
    {
        isRotationEnabled = false;
        isRotationForChildEnabled = true;
        isMoveEnabled = false;
    }

    /// <summary>
    /// Enables the movement of the object by setting the variables accordingly
    /// </summary>
    public void EnableMove()
    {
        isRotationEnabled = false;
        isRotationForChildEnabled = false;
        isMoveEnabled = true;
    }
}
