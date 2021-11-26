using UnityEngine;

/// <summary>
/// While placing the prop, the prop is transparent. This is helpful to visualize the placing process. If the prop is placed, the transparency disappears.
/// For that, the materials of each prop are stored in 'originalMaterial'
/// </summary>
public class Transparency : MonoBehaviour
{
    #region Init

    private Material[] originalMaterial;
    public Material newMaterial;
    private Material[] changedMaterial;
    private bool isInitialized = false;
    public bool restoreOldMaterials = false;
    public HoloToolkit.Unity.SpatialMapping.TapToPlaceCustom tapToPlaceCustomScript;
    private Rigidbody ownRigidBody;
    private float velocity;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    private void Start()
    {
        init();
    }

    /// <summary>
    /// Find the original materials and store them in "originalMaterial"
    /// It must initialize itself before the Update method can work, otherwise it is too fast for the copy process
    /// </summary>
    void init()
    {
        /*
         * Velocity is an additional flag, to check if the Prop is moving unintentionally or not (e.g. a hole in the mesh) 
         */

        velocity = 0f;
        ownRigidBody = tapToPlaceCustomScript.gameObject.GetComponent<Rigidbody>();

        /*
         * The Props are from the App Store, and from different sellers. This causes different structures and layers within the Prop.
         * In this case, the renderer for the Prop are of different kinds, so it has to be testet before copying the materials 
         */

        if (GetComponent<MeshRenderer>())
        {
            originalMaterial = GetComponent<MeshRenderer>().materials;
        }
        else if (GetComponent<SkinnedMeshRenderer>())
        {
            originalMaterial = GetComponent<SkinnedMeshRenderer>().materials;
        }

        /*
         * Store the original material(s) in changedMaterial[], for having the possibility of restoring them later
         */

        changedMaterial = new Material[originalMaterial.Length];
        for (int a = 0; a < originalMaterial.Length; a++)
        {
            changedMaterial[a] = newMaterial;
        }

        /*
         * Set isInitialized to true, to activate the Update method
         */

        isInitialized = true;
    }

    /// <summary>
    /// Always checks, in which state a prop is. if something is wrong with the prop or it is in the state of placing/replacing, it will change to transparent.
    /// </summary>
    private void Update()
    {
        Vector3 tempVelocity = tapToPlaceCustomScript.gameObject.GetComponent<Rigidbody>().velocity;
        velocity = tempVelocity.x + tempVelocity.y + tempVelocity.z;

        /*
         * Stop Update method as long as the Init process needs
         */

        if (isInitialized)
        {

            /*
             * Test Prop for existing "tapToPlaceCustomScript".
             * Otherwise it could result in a crash
             */

            if (tapToPlaceCustomScript)
            {
                if (tapToPlaceCustomScript.propIsInHand == false && tapToPlaceCustomScript.IsBeingPlaced == false
                    && ownRigidBody.useGravity == false && ownRigidBody.isKinematic == true && velocity == 0f
                    )
                {
                    /* At the beginning of the placement, the Object-Tag is changed. This is the signal, that the placement-process is not ready
                     * After the placing, the tag name is changed back to AVP_Object
                     */

                    if (transform.tag != "MaterialPosition")
                    {
                        transform.tag = "AVP_Object";
                    }
                    if (GetComponent<MeshRenderer>())
                    {
                        if (GetComponent<MeshRenderer>().materials != originalMaterial)
                        {
                            GetComponent<MeshRenderer>().materials = originalMaterial;
                        }
                    }
                    else if (GetComponent<SkinnedMeshRenderer>())
                    {
                        if (GetComponent<SkinnedMeshRenderer>().materials != originalMaterial)
                        {
                            GetComponent<SkinnedMeshRenderer>().materials = originalMaterial;
                        }
                    }
                }
                else
                {
                    /* Objext can not be saved in this state and is also transparent.
                     * The User has a visible feedback.
                     */

                    if (transform.tag != "MaterialPosition")
                    {
                        transform.tag = "AVP_Object_Moving";
                    }
                    if (GetComponent<MeshRenderer>())
                    {
                        GetComponent<MeshRenderer>().materials = changedMaterial;
                    }
                    else if (GetComponent<SkinnedMeshRenderer>())
                    {
                        GetComponent<SkinnedMeshRenderer>().materials = changedMaterial;
                    }
                }
            }
        }
    }
}
