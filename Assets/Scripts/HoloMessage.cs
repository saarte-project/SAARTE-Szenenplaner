using UnityEngine;
using UnityEngine.UI;

namespace Saarte
{
    /// <summary>
    /// If this script is added to another class, messages can be sent directly to the hololens.
    /// This can be useful if an important message for the user has to be displayed or, in case of an issue, a debug message.
    /// </summary>
    public class HoloMessage : MonoBehaviour
    {
        #region Init

        private bool disappear = false;
        private float waitTime = 0;

        [Tooltip("The connected Textfield. A child of the Hololens Camera.")]
        private Text textField;

        #endregion

        /// <summary>
        /// Sends a message directly to the hololens, fixed color
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="messageTime">The time before the message disappears</param>
        public void setMessage(string message, float messageTime)
        {
            StopAllCoroutines();
            setDebugMessageOnDisplay(message, new Color(255,0,0));
            waitTime = messageTime;
            disappear = true;
        }

        /// <summary>
        /// Sends a message directly to the hololens, color can be changed.
        /// Simple message for the user
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="c">The color in Hex.</param>
        /// <param name="messageTime">The time before the message disappears</param>
        public void setMessage(string message, float messageTime,Color c)
        {
            setDebugMessageOnDisplay(message, c);
            waitTime = messageTime;
            disappear = true;
        }

        /// <summary>
        /// Sends a message directly to the hololens, color can be changed.
        /// Debug message for the developer
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="c">The color in Hex.</param>
        /// <param name="messageTime">The time before the message disappears</param>
        public void setDebugMessage(string debugMessage, float messageTime, Color c)
        {
            setDebugMessageOnDisplay(debugMessage, c);
            waitTime = messageTime;
            disappear = true;
        }

        /// <summary>
        /// Sends a message for an infinite time directly to the hololens, color can be changed, infinite time
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// <param name="c">The color in Hex.</param>
        private void setDebugMessageOnDisplay(string message,Color newColor)
        {
            textField = GetComponent<Text>();
            textField.GetComponent<Text>().color = new Color(newColor.r, newColor.g, newColor.b);
            textField.GetComponent<Text>().text = message;         
        }

        /// <summary>
        /// Interrupts the message immediately
        /// </summary>
        /// <param name="message">The message as a string.</param>
        /// /// <param name="c">The color in Hex.</param>
        public void interruptMessage()
        {
            textField.GetComponent<Text>().text = "";
        }

        /// <summary>
        /// Cares for the time the message shhould be visible
        /// </summary>
        private void Update()
        {
            if(disappear)
            {
                if (waitTime >= 0)
                {
                    waitTime -= Time.deltaTime;
                }
                else
                {
                    textField.GetComponent<Text>().text = "";
                    disappear = false;       
                }
            }
        }
    }
}
