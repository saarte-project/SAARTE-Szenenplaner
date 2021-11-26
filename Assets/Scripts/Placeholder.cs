using UnityEngine;
using UnityEngine.UI;

namespace SAARTE
{
    /// <summary>
    /// Overrides the text input field in the stroyboard menu to show a hint
    /// If the user uses the virtual keyboard, the hint will disappear
    /// </summary>
    public class Placeholder : MonoBehaviour
    {
        private Text text;
        public GameObject placeHolder;
       
            void Update()
        {
            if (transform.name == "TextForStoryBoard")
            {
                if (GetComponent<Text>().text == "" || GetComponent<Text>().text == null)
                {
                    placeHolder.GetComponent<Text>().text = " Name eingeben ";
                }
                else
                {
                    placeHolder.GetComponent<Text>().text = "";
                    
                }
            }
            else if (transform.name == "Note")
            {
                if (GetComponent<Text>().text == "" || GetComponent<Text>().text == null)
                {
                    placeHolder.GetComponent<Text>().text = "- Notizen -";
                }
                else
                {
                    placeHolder.GetComponent<Text>().text = "";
                    
                }
            }
        }
    }
}
