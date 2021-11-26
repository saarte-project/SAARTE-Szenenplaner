using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Text field that shows the text only for a certain amount of time
/// </summary>
public class TextDisplayForTime : MonoBehaviour
{
    #region Init

    public float timeTextIsDisplayed = 5;
    private float timeWaited;
    private Text textField;

    #endregion

    /// <summary>
    /// Start setup
    /// </summary>
    void Start ()
    {
        textField = transform.GetComponent<Text>();
        timeWaited = 0.5f;
    }

    /// <summary>
    /// A text field with a time span to live
    /// </summary>
    public void DisplayText(string textToDisplay)
    {
        textField.text = textToDisplay;
        StartCoroutine(DeleteTextAfterTime(timeWaited));
    }

    /// <summary>
    /// Logic for activation and deactivation of a text
    /// </summary>
    IEnumerator DeleteTextAfterTime(float waitTime)
    {       
        yield return new WaitForSeconds(waitTime);
        textField.text = "";
        yield return null;
    }
}
