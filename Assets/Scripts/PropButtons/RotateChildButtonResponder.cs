using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The behavior of the button allowing to rotate the top parts of props in the scene.
/// </summary>
public class RotateChildButtonResponder : PropButton
{
    private bool rotationButtonActive;

    protected override void PropButtonStart()
    {
        rotationButtonActive = false;
    }

    public override void OnInputClicked(InputClickedEventData eventData)
    {
        Transform[] children = gameObject.transform.parent.GetComponentsInChildren<Transform>();

        // change the color of non-pressed button in default color
        if ( !rotationButtonActive )
        {
            foreach (Transform item in children)
            {
                if (item.name == StaticVariables.nameButtonMove)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    item.transform.GetChild(0).GetComponent<Text>().color = Color.white;

                }
                else if (item.name == StaticVariables.nameButtonRotate)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    item.transform.GetChild(0).GetComponent<Text>().color = Color.white;
                }
            }
            GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            transform.GetChild(0).GetComponent<Text>().color = Color.black;
            gameObject.transform.parent.gameObject.GetComponentInParent<GestureAction>().EnableChildRotation();
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
