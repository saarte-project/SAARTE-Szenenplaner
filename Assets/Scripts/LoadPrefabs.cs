using UnityEngine;

/// <summary>
/// At startup: In Prefab InitializationOfSceneAndScripts
/// Controls the way, the first prefabs will spawn into the scene
/// </summary>
public class LoadPrefabs : MonoBehaviour
{
    #region Init

    public GameObject visualKeyboard;

    #endregion

    /// <summary>
    /// On App start - activate several Gameobjects
    /// </summary>
    void Start ()
    {       
        visualKeyboard.SetActive(true);       
    } 
}
