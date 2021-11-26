using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The behavior of the button allowing to reposition a prop in the scene.
/// </summary>
public class ReplaceButton : PropButton
{
    protected override void PropButtonStart()
    {
        /*
         * No action needad
         */
    }
    public override void OnInputClicked(InputClickedEventData eventData)
    {
        Transform[] children = gameObject.transform.parent.GetComponentsInChildren<Transform>();

        // change the color of non-pressed button in default color
        foreach (Transform item in children)
        {
            if (item.name == StaticVariables.nameButtonRotate)
            {
                item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                item.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            }
            else if (item.name == StaticVariables.nameButtonRotateChild)
            {
                item.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            }
        }
        gameObject.transform.parent.gameObject.GetComponentInParent<TapToPlaceCustom>().ButtonStartPlacement();
        eventData.Use();
    }
}
