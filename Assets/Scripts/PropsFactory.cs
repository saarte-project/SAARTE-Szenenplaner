using HoloToolkit.UI.Keyboard;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

/// <summary>
/// The purpose of this class is to create the object (e.g. "Requisiten") into the spawn points and to add the needed components.
/// Added components or changed variables:
/// RigidBody
/// Buttons-Prefab
/// Scale
/// Scripts: GestureAction, Selectable
/// sets the GlobalScaler as parent
/// </summary>
public class PropsFactory : MonoBehaviour
{
    /// <summary>
    /// The prefab for the buttons for the object wich will be added if the object has a child named "Head".
    /// </summary>
    public GameObject buttonWithChildPrefab;

    /// <summary>
    /// The prefab for the buttons to be added to the object.
    /// </summary>
    public GameObject buttonPrefab;

    /// <summary>
    /// The object/ prefab which has been spawned
    /// </summary>
    private GameObject spawnedObject;

    string textFieldPosition;

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        textFieldPosition = "NoticeboardCanvas/NoticeboardBackground/NoticeboardText";
    }

    /// <summary>
    /// The "SpawnerHelper" instantiates Props, which have been sent by one of the three overloaded SpawnObject functions.
    /// </summary>
    /// <param name="isLoaded">Is the Prop loaded from file or instantiated at runtime</param>
    /// <param name="panelName">The name of the Panel</param>
    /// <param name="cellNumber">The cell where the prop remains</param>
    /// <param name="spawnObject">The prop itself</param>
    /// <param name="buttons">The added buttons</param>
    /// <param name="isBeingPlaced">Is the prop placed or not</param>
    /// <param name="hasHead">Does the prop have an extra functionality? In this case a head</param>
    /// <param name="noteText">the text wich is added onto the noticeboard</param>
    private void SpawnerHelper(bool isLoaded,string panelName, int cellNumber, GameObject spawnObject, ref GameObject buttons, bool isBeingPlaced, bool hasHead,
        string noteText = "",
        Vector3 spawnPosition = new Vector3(), Vector3 spawnRotation = new Vector3(),
        Vector3 spawnChildPosition = new Vector3(), Vector3 spawnChildRotation = new Vector3())
    {
        /*
        A new instance of a prop will be created and anchored at its proper container. The reference number (cellNumber) for the new instance has been taken by the
        script 'Count Cells', which is responsible for creating ans storing a unique number for all panels and which is located at the 'Panel Content' GameObject.
        The triggers for the instantiation are buttons on the top of the 'Panel Menu', which refer to the function 'SpawnObject'.
        The chosen prop, its values and the matching cell number will be sent to the 'SpawnObject' function, which is located in this script.
        */

        /*
         * Instantiate the Prop in the proper container. The container can be located by the cellNumber, which is a part of the functions members 
         */

        spawnedObject = Instantiate(spawnObject, GameObject.Find("_Instance.Container_" + cellNumber.ToString()).transform) as GameObject;
        spawnedObject.transform.name += panelName;
        

        /*
         * Is the prop loaded or instantiated by the user?
         */

        if(isLoaded)
        {
            spawnedObject.transform.localEulerAngles = spawnRotation;
        }
        
        /*
         * Is the prop still in 'setup' mode?
         */

        if (isBeingPlaced)
        {
            spawnedObject.transform.localPosition = spawnPosition;
            spawnedObject.transform.localEulerAngles = spawnRotation;
        }

        /* 
         * searches the child named "Head" and adds the button with or without the RotateHead button, depending if a head exists.
         * IMPORTANT: Do not change the names of the Prop parts, they are used for referencing and finding them in the scene
         */

        if (spawnedObject.transform.Find("Head"))
        {
            buttons = Instantiate(buttonWithChildPrefab, spawnedObject.transform);
            if (hasHead)
            {
                spawnedObject.transform.Find("Head").localPosition = spawnChildPosition;
                spawnedObject.transform.Find("Head").localEulerAngles = spawnChildRotation;
                spawnedObject.transform.localPosition = spawnPosition;
                spawnedObject.transform.localEulerAngles = spawnRotation;
            }
        }
        else
        {
            buttons = Instantiate(buttonPrefab, spawnedObject.transform);
        }

        buttons.transform.localScale = StaticVariables.propsMenuButtonsSize;
        buttons.transform.localPosition = spawnedObject.GetComponent<AnchorForButtons>().GetAnchorPosition();      
        var textField = buttons.transform.Find(textFieldPosition).GetComponent<KeyboardInputField>();
        textField.text = noteText;
        spawnedObject.transform.localPosition = spawnPosition;
        spawnedObject.transform.localEulerAngles = spawnRotation;
    }

    /// <summary>
    /// Spawns a simple object in a spawnpoint and adds the necessary components to let a user place the object by gaze.
    /// </summary>
    /// <param name="spawnObject">the prefab which is going to be spawned</param>
    /// <param name="isPropLoadedFromFile">Is prop loaded or instantiated by the user</param>
    /// <param name="cellNumber">The panel, where the prop should remain.</param>
    /// <param name="isLoaded">is the new prop loaded from file or instantiated in runtime</param>
    /// <param name="panelName">The name of the Panel</param>
    public GameObject SpawnObject(GameObject spawnObject, bool isPropLoadedFromFile, int cellNumber, string panelName)
    {
        GameObject buttons = null;
        const bool isBeingPlaced = true;
        const bool hasHead = false;
        const bool isLoaded = false;
        SpawnerHelper(isLoaded,panelName, cellNumber, spawnObject, ref buttons, isBeingPlaced, hasHead);
        SetPresets(isPropLoadedFromFile, buttons);
        return spawnedObject;
    }

    /// <summary>
    /// Spawns a Prop without Head into a Container and adds the necessary components.
    /// IMPORTANT: Is called, if a Prop is loaded from file
    /// </summary>
    /// <param name="spawnObject">the Prefab wich is beeing spawned</param>
    /// <param name="spawnPosition">the position at wich is should be spawned</param>
    /// <param name="spawnRotation">the rotation at wich is should be spawned</param>
    /// <param name="noteText">the text wich is added onto the noticeboard</param>
    /// <param name="isPropLoadedFromFile">is the new prop loaded from file or instantiated in runtime</param>
    /// <param name="cellNumber">The number of the Panel</param>
    /// <param name="panelName">The name of the Panel</param>
    public void SpawnObject(GameObject spawnObject, Vector3 spawnPosition, Vector3 spawnRotation, string noteText, bool isPropLoadedFromFile, int cellNumber,string panelName)
    {
        GameObject buttons = null;
        const bool isBeingPlaced = false;
        const bool hasHead = false;
        const bool isLoaded = true;
        SpawnerHelper(isLoaded,panelName, cellNumber, spawnObject, ref buttons, isBeingPlaced, hasHead, noteText, spawnPosition, spawnRotation);
        SetPresets(isPropLoadedFromFile, buttons);
    }

    /// <summary>
    /// Spawns an object with a head in a spawnpoint and adds the necessary components.
    /// IMPORTANT: Is called, if a Prop is loaded from file
    /// </summary>
    /// <param name="spawnObject">the Prefab wich is beeing spawned</param>
    /// <param name="spawnPosition">the position at wich is should be spawned</param>
    /// <param name="spawnRotation">the rotation at wich is should be spawned</param>
    /// <param name="spawnChildPosition">the position at wich the Head of the object is spawned</param>
    /// <param name="spawnChildRotation">the rotation at wich the Head of the object is spawned</param>
    /// <param name="noteText">the text wich is added onto the noticeboard</param>
    /// <param name="isPropLoadedFromFile">is the new prop loaded from file or instantiated in runtime</param>
    /// <param name="cellNumber">The number of the Panel</param>
    /// <param name="panelName">The name of the Panel</param>
    public void SpawnObject(GameObject spawnObject, Vector3 spawnPosition, Vector3 spawnRotation, Vector3 spawnChildPosition, Vector3 spawnChildRotation, string noteText, bool isPropLoadedFromFile, int cellNumber,string panelName)
    {
        GameObject buttons = null;
        const bool hasHead = true;
        const bool isBeingPlaced = false;
        const bool isLoaded = true;
        SpawnerHelper(isLoaded,panelName, cellNumber, spawnObject, ref buttons, isBeingPlaced, hasHead, noteText, spawnPosition, spawnRotation, spawnChildPosition, spawnChildRotation);
        SetPresets(isPropLoadedFromFile, buttons);
    }

    /// <summary>
    /// Sets several presets for the instantiated Prop  
    /// </summary>
    /// <param name="isPropLoadedFromFile">If a prop is loaded from file, it has to be placed on the proper position. Otherwise is would stick in front of the user, waiting to get placed by a mouse click.</param>
    public void SetPresets(bool isPropLoadedFromFile, GameObject buttons)
    {
        var selectableScript = spawnedObject.AddComponent<Selectable>();
        selectableScript.buttons = buttons;
        buttons.gameObject.SetActive(false);
        spawnedObject.transform.tag = StaticVariables.objectTag;
        if (isPropLoadedFromFile == false)
        {
            spawnedObject.GetComponent<TapToPlaceCustom>().IsBeingPlaced = true;
            spawnedObject.GetComponent<TapToPlaceCustom>().propIsInHand = true;
        }
        else
        {
            spawnedObject.SetActive(false);
        }
    }
    
}
