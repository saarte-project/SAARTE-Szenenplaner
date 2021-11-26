using HoloToolkit.Unity.InputModule;
using UnityEngine;

/// <summary>
/// The behavior of the button destroying props in the scene.
/// </summary>
public class DestroyObject : PropButton
{
    protected override void PropButtonStart()
    {
    }

    public override void OnInputClicked(InputClickedEventData eventData)
    {
        Destroy(transform.parent.parent.gameObject);
        eventData.Use();
    }
}
