using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The behavior of the button allowing to take notes for props in the scene.
/// </summary>
public class NoteButton : PropButton
{
    public GameObject Object;

    protected override void PropButtonStart()
    {
    }
    public override void OnInputClicked(InputClickedEventData eventData)
    {
        Object.SetActive(!Object.activeSelf);

        if (Object.activeSelf)
        {
            transform.GetComponent<Renderer>().material.color = Color.green;
            transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
        else
        {
            transform.GetComponent<Renderer>().material.color = Color.black;
            transform.GetChild(0).GetComponent<Text>().color = Color.white;
        }
        eventData.Use();
    }
}
