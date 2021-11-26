using UnityEngine;

/// <summary>
/// Encoding Textures to byte [] is only possible on the main threat. So saving pictures to disc causes delays at runtime. 
/// To avoid this, the picture will be 'Auto-Encoded' while the photo is taken, and stored as a byte array inside the panel-object.
/// Next time, the picture-saving process has access to the 'byteArray' and can perform the save-process very quickly and without any disturbances. 
/// </summary>
public class JPGtoByte : MonoBehaviour
{
    #region Init

    private byte[] byteArray = { };
    public int byteArraySize = 0;

    #endregion

    /// <summary>
    /// Changes the value of the byteArraySize permanently, depending on the data
    /// </summary>
    private void Update()
    {
        byteArraySize = byteArray.Length;
    }

    /// <summary>
    /// Encode Texture2D in a jpg and also to a byte array
    /// </summary>
    public void SetByteArray(Texture2D t)
    {
        /*
         * Encode Texture 2D back to a jpg and store it as a byte array. 
         * The new byte array will be stored with the related panel.
         * Advantage: While the saving process is performing save after save, no more time for encoding 
         */

        byteArray = t.EncodeToJPG();
        byteArraySize = byteArray.Length;
    }

    /// <summary>
    /// Returns the 'ready to save' byteArray 
    /// </summary>
    public byte[] GetByteArray()
    {
        return byteArray;
    }

    /// <summary>
    /// Returns the size of the byte array
    /// </summary>
    public int GetByteArraySize()
    {
        return byteArraySize;
    }
}
