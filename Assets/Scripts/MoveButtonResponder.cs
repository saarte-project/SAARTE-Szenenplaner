using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class MoveButtonResponder calls the function "OnClickChildButton"
/// of the script GestureAction with the parameter "Move Object", 
/// if the button "Move" is pressed. 
/// </summary>
public class MoveButtonResponder : MonoBehaviour, IInputClickHandler
{ 
    /// <summary>
    /// Handles click of move button
    /// </summary>
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Transform[] children = gameObject.transform.parent.GetComponentsInChildren<Transform>();

        if (gameObject.GetComponent<Renderer>().material.color == Color.black)
        {
            /*  
             * Change the color of non-pressed button in default color
             */

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

            GetComponent<Renderer>().material.SetColor("_Color", Color.blue);

            gameObject.transform.parent.gameObject.GetComponentInParent<GestureAction>().EnableMove();
        }
        else if (gameObject.GetComponent<Renderer>().material.color != Color.black)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            gameObject.transform.parent.gameObject.GetComponentInParent<GestureAction>().EnableMove();

            foreach (Transform item in children)
            {
                if (item.name == StaticVariables.nameButtonMove)
                {
                    item.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                }
            }
            
        }

        eventData.Use();
    }
}
