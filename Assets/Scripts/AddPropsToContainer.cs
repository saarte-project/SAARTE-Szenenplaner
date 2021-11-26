using System.Collections;
using UnityEngine;

/// <summary>
/// This class is responsible for creating a Gameobject as a child of the PropsSpawnContainer. To this GameObject '_Instance.Container_',
/// the number of the matching Container is added at the end of its name. 
/// The moment, the user instantiates a prop, the prop will be anchored as a child of the proper Instance_Container.
/// </summary>
public class AddPropsToContainer : MonoBehaviour {

    #region Init

    public GameObject propsSpawnContainer;
    private GameObject propsSelf;
    public GameObject propsDummy;

    #endregion

    /// <summary>
    /// Create a new Gameobject and name it "PropsContainer" in the first step.
    /// The container is used to store the new Instances of the Props 
    /// </summary>
    private void Start()
    {
        propsSelf = Instantiate(propsDummy, propsSpawnContainer.transform) as GameObject;
        propsSelf.tag = "PropsContainer";
        StartCoroutine(CheckForCloneDummy());
    }

    /// <summary>
    /// Each frame check for the own name. Change name of the Props container, if the own name has changed
    /// </summary>
    void Update () {
        propsSelf.name = "_Instance."+transform.name;
    }

    /// <summary>
    /// This class is responsible for creating a Gameobject as a child of the PropsSpawnContainer. To this GameObject '_Instance.Container_',
    /// the number of the matching container is added at the end of its name. 
    /// The moment, the user instantiates a prop, the prop will be anchored as a child of the proper Instance_Container.
    /// </summary>
    /// <param name="panelNumber">The proper panel number, to find the referenced GameObject</param>
    public void DestroyPropSelf(int panelNumber)
    {
        GameObject containerToDestroy = propsSpawnContainer.transform.Find("_Instance.Container_" + panelNumber).gameObject;
        Destroy(containerToDestroy);
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// Destroyes all existing Props in a singel container
    /// </summary>
    /// <param name="panelNumber">The proper panel number, to find the referenced GameObject</param>
    public void DestroyPropsInSingleContainer(int panelNumber)
    {
        GameObject propsToDestroy = propsSpawnContainer.transform.Find("_Instance.Container_" + panelNumber).gameObject;
        for(int a=0; a < propsToDestroy.transform.childCount; a++)
        {
            GameObject tempChild = propsToDestroy.transform.GetChild(a).gameObject;
            Destroy(tempChild);
        }
    }

    /// <summary>
    /// In an older version a copy of a clone appeared in the Spawn container. This is the deletion function for it.
    /// </summary>
    IEnumerator CheckForCloneDummy()
    {
        yield return new WaitForSeconds(0.5f);
        for(int a = 0; a < propsSpawnContainer.transform.childCount; a++)
        {
            if(propsSpawnContainer.transform.GetChild(a).name =="CloneDummy(Clone)")
            {
                Destroy(propsSpawnContainer.transform.GetChild(a).gameObject);
            }
        }
        yield return null;
    }
}
