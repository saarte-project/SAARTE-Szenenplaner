using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The behavior of the button allowing to rotate props in the scene.
/// </summary>
public class RotateButtonResponder : PropButton
{
    /// Indicates if the button is toggled and thus a rotation is in action
    private bool rotationButtonActive;

    protected override void PropButtonStart()
    {
        rotationButtonActive = false;
    }

    public override void OnInputClicked(InputClickedEventData eventData)
    {
        // Get all the children of the parent object 
        Transform[] children = gameObject.transform.parent.GetComponentsInChildren<Transform>();

        // Change the color of non-pressed button in default color
        if( !rotationButtonActive )
        {
            foreach (Transform item in children)
            {
                if (item.name == StaticVariables.nameButtonMove)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    item.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                }
                else if (item.name == StaticVariables.nameButtonRotateChild)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    item.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                }
            }
            gameObject.transform.parent.gameObject.GetComponentInParent<GestureAction>().EnableRotation();
            GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            transform.GetChild(0).GetComponent<Text>().color = Color.white;
            gameObject.transform.parent.gameObject.GetComponentInParent<GestureAction>().EnableMove();
            foreach (Transform item in children)
            {
                if (item.name == StaticVariables.nameButtonMove)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                }
            }
        }
        rotationButtonActive = !rotationButtonActive; // toggle rotationActive
        eventData.Use();
    }
}
