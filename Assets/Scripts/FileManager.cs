using HoloToolkit.UI.Keyboard;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Assertions;
using Saarte;
using System;
using System.Linq;

/// <summary>
/// Manages save, load and delete of the storyboard.
/// Also updates the list of buttons in the menu after something changed.
/// </summary>
public class FileManager : MonoBehaviour
{
    #region Init

    public Texture2D defaultTexture2D;
    public GameObject loadedFileButton;
    private static string textEditFieldName = "Menu/StoryBoardBackground/StoryBoardName/TextForStoryBoard";
    private static string noteFieldName = "NoticeBackground/Note";
    private GameObject anchorForPanelInstances;
    private GameObject panelListCanvas;
    public GameObject cellClone;
    private bool savingIsActive;
    private const string UNNAMED = "Unbenannt";
    private const string AUTOSAVE = "Autosave";
    private const string FLAGFILESUFFIX = "%%%%";
    private float newX, newY, newWidth, newHeight;
    private GameObject holoMessage;
    GameObject menus;
    private int backupCounter;

    private bool storyBoardSaved;
    private bool propsSaved;

    #endregion

    #region Init

    /// <summary>
    /// Decleration and Gameobjects to find
    /// </summary>
    void Start()
    {
        storyBoardSaved = false;
        propsSaved = false;
        holoMessage = GameObject.FindGameObjectWithTag("DebugHoloLens");
        savingIsActive = false;
        cellClone.SetActive(false);
        GameObject storyBoardMenu = GameObject.FindGameObjectWithTag("StoryBoardMenu");
        gameObject.transform.SetParent(storyBoardMenu.transform);
        menus = GameObject.FindGameObjectWithTag("Menus");
        menus.GetComponent<ActivateLoadMenu>().InitializeDeactivated();
        newX = 0.25f;
        newY = 0.25f;
        newHeight = 0.45f;
        newWidth = 0.45f;
        backupCounter = 0;
        anchorForPanelInstances = GameObject.FindGameObjectWithTag("Image");

        if (!Directory.Exists(StaticVariables.savedFilepath))
        {
            Directory.CreateDirectory(StaticVariables.savedFilepath);
        }
        panelListCanvas = GameObject.FindGameObjectWithTag("PanelListCanvas");
        StaticVariables.activeStoryBoard = UNNAMED;
        ResetFileName("");
    }

    #endregion

    #region Helper Functions

    /// <summary>
    /// After a Load/Save/Delete/Reset of a Storyboard, the Name of the file must be reset to avoid issues
    /// </summary>
    private void ResetFileName(string newName)
    {
        StaticVariables.fileName = newName;
    }

    /// <summary>
    /// In case of deletion all Files and thus the directory will be deleted
    /// </summary>
    /// <param name="storyboardPath">The path for the file.</param>
    private bool DeleteAllFiles(string storyboardPath)
    {
        /*
         * Delete all existing files before the saving process starts. 
         */

        if (!Directory.Exists(StaticVariables.savedFilepath + storyboardPath))
        {
            Directory.CreateDirectory(StaticVariables.savedFilepath + storyboardPath);
        }

        DirectoryInfo di = new DirectoryInfo(StaticVariables.savedFilepath + storyboardPath);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }

        return true;
    }

    /// <summary>
    /// Instantiates as many buttons as saved storyboards are available 
    /// The name of the file later is the text component of the button
    /// </summary>
    public void UpdateAllFiles()
    {
        /*
         * Destroy old buttons
         */

        GameObject[] allButtons = GameObject.FindGameObjectsWithTag("LoadSaveButton");
        for (int a = 0; a < allButtons.Length; a++)
        {
            Destroy(allButtons[a]);
        }
        Resources.UnloadUnusedAssets();

        /*
         * Check for Autosave file in folder and check Flag file. Collect all available Folders which contain a Flag file
         */

        List<DateTime> timeStamps = new List<DateTime>();
        int autoSaveFileCounter = 0;
        int allOtherFileCounter = 0;
        DateTime tempCreationTime = new DateTime();
        Transform list = transform.Find("Menu/ScrollView/Panel/List");
        string[] savedFiles = Directory.GetDirectories(StaticVariables.savedFilepath, "*");
        string[] allAutosaveDirectories = new string[savedFiles.Length];
        string[] allOtherDirectories = new string[savedFiles.Length];
        if (savedFiles.Length > 0)
        {
            loadedFileButton.SetActive(true);
            for (int t = 0; t < savedFiles.Length; t++)
            {
                if (savedFiles[t].Contains(AUTOSAVE))
                {
                    allAutosaveDirectories[autoSaveFileCounter] = savedFiles[t];
                    tempCreationTime = CheckAutoSaveFilesForCompleteness(savedFiles[t]);
                    if (tempCreationTime.Hour != 0)
                    {

                        /*
                         * Add all timestamps to a List
                         */

                        timeStamps.Add(tempCreationTime);
                        autoSaveFileCounter++;
                    }                  
                }
                else
                {
                    allOtherDirectories[allOtherFileCounter] = savedFiles[t];
                    allOtherFileCounter++;
                }
            }
        }

        if (autoSaveFileCounter > 0)
        {

            /*
             *  Sort List to get a List of files, sorted by their creationDate
             */

            timeStamps.Sort((a, b) => a.CompareTo(b));

            /*
             * Get the proper name of the directory
             */

            string[] autosaveFolderAndName = CompareCreationDateFromAutosave(allAutosaveDirectories, timeStamps.Last<DateTime>());

            /*
             * Instantiate ONE Autosave button if a Autosave File exists and is complete
             */

            if (autosaveFolderAndName[0] != "EMPTY" && autosaveFolderAndName[1] != "EMPTY")
            {
                GameObject AutosaveButton = Instantiate(loadedFileButton, list.transform);
                AutosaveButton.GetComponentInChildren<Text>().text = AUTOSAVE;
                AutosaveButton.GetComponent<SelectableButtons>().SetFolderForAutosave(autosaveFolderAndName[0]);
                AutosaveButton.GetComponent<SelectableButtons>().SetNameForAutosave(autosaveFolderAndName[1]);
            }
        }

        for (int i = 0; i < allOtherDirectories.Length; i++)
        {                     
            if (allOtherDirectories[i] != null)
            {
                GameObject tempButton = Instantiate(loadedFileButton, list.transform);
                tempButton.GetComponentInChildren<Text>().text = allOtherDirectories[i].Remove(0, Path.GetDirectoryName(allOtherDirectories[i]).Length + 1);
                string tempText = tempButton.GetComponentInChildren<Text>().text;
            }
        }
    }

    /// <summary>
    /// Backward check (Time Stamp) of Autosave files. A "Ready" file must be a part of the Autosave folder, only then it will be accepted. Otherwise there will be no Autosave File 
    /// A button with the name "Autosave" will be generated with the reference to the accepted Autosave folder.
    /// </summary>
    /// <param name="name">The name of the directory to find.</param>
    private DateTime CheckAutoSaveFilesForCompleteness(string name)
    {
        DirectoryInfo di = new DirectoryInfo(name);
        FileInfo[] Files = di.GetFiles();
        DateTime fileCreationTime = new DateTime();
        foreach (FileInfo file in Files)
        {
            if (file.Name.Contains(FLAGFILESUFFIX))
            {
                fileCreationTime = file.CreationTimeUtc;
            }
        }
        return fileCreationTime;
    }
   
    private string[] CompareCreationDateFromAutosave(string[] allDirs, DateTime foundDate)
    {
        string[] foundDirectoryAndName = new string[2];
        bool foundMatchingFile = false;
        foreach (string dir in allDirs) {
            if (dir != null)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                FileInfo[] Files = di.GetFiles();

                foreach (FileInfo file in Files)
                {
                    if (file.CreationTimeUtc.Equals(foundDate))
                    {
                        foundDirectoryAndName[0] = file.DirectoryName;
                        string tempFileName = file.Name;
                        int tempFileNameLength = tempFileName.Length;
                        foundDirectoryAndName[1] = tempFileName.Remove(tempFileNameLength - FLAGFILESUFFIX.Length);
                        foundMatchingFile = true;
                    }
                }
            }
        }
        if (foundMatchingFile)
        {
            return foundDirectoryAndName;
        }
        else
        {
            foundDirectoryAndName[0] = "EMPTY";
            foundDirectoryAndName[1] = "EMPTY";
            return foundDirectoryAndName;
        }
    }

    /// <summary>
    /// Destroys all panels before a new Storyboard can be loaded
    /// </summary>
    /// <param name="activePanels">All Panels in the scene</param>
    private bool WaitForPanelDestruction(GameObject[] activePanels)
    {
        for (int a = 0; a < activePanels.Length; a++)
        {
            Destroy(activePanels[a]);
        }
        Resources.UnloadUnusedAssets();
        return true;
    }

    /// <summary>
    /// Deactivates every active cellbackground
    /// </summary>
    private void DeactivatePanelHighlight()
    {
        // Deactivate the highlighting af all panels
        GameObject[] allBackgrounds;
        allBackgrounds = GameObject.FindGameObjectsWithTag("CellBackground");
        for (int a = 0; a < allBackgrounds.Length; a++)
        {
            Image tmpImage = allBackgrounds[a].GetComponent<Image>();
            Assert.IsNotNull(tmpImage, "Object does not have expected Image component.");
            tmpImage.enabled = false;
        }
    }

    /// <summary>
    /// Sets a number for each panel. Panels start from 1 up to n. 
    /// This numbers are stored in the 'Panel Content' GameObject, which is part of the Storyboard
    /// </summary>
    /// <param name="waitTime">Waits a time to be asure, that a panels is deleted, so that no wrong calculation can be performed</param>
    /// <param name="isAutoSave">A boolean as an information for the function 'SaveProps' which is called later on</param>
    IEnumerator ReorganizePanelNumbersAndWait(float waitTime, bool isAutoSave)
    {
        /*
         * Waits n seconds to be sure that the save/load/delete process has been finished
         */

        yield return new WaitForSeconds(waitTime);

        /*
         *  Reorganize numbering of all active panels in the scene
         */

        GameObject[] allPanels = GameObject.FindGameObjectsWithTag("Cell");
        for (int a = 0; a < allPanels.Length; a++)
        {
            allPanels[a].GetComponentInChildren<CountCells>().SetPanelNumber(a + 1);
        }

        /*
         * Reorganizes Add button position and Background image size
         */
        panelListCanvas.GetComponent<PanelManager>().OnLoadButtonPressed();
        yield return null;
    }

    /// <summary>
    /// Stores the pre-converted binary file of an image in the JPGtoByte script, which is part of the storyboard panel
    /// </summary>
    /// <param name="loadTex">The texture, which will be converted into a byte array</param>
    /// <param name="currentChild">The actual storyboard panel</param>
    IEnumerator SetByteArray(Transform currentChild, Texture2D loadTex)
    {
        /*
         * Encode Texture 2D back to a jpg and store it as a byte array. 
         * The new byte array will be stored with the related panel.
         * Advantage: While the saving process is performing save after save, no more time for encoding 
         */

        currentChild.transform.GetChild(0).GetComponent<JPGtoByte>().SetByteArray(loadTex);
        yield return true;
    }

    #endregion

    #region Buttons

    /// <summary>
    /// This function is called after clicking the load button in the Storyboard Menu
    /// </summary>
    public void OnLoadButtonClick()
    {
        LoadStoryboardAfterClick();
    }

    /// <summary>
    /// This function is called after a click on the save button (Storyboard Menu)
    /// </summary> 
    public void OnSaveButtonClick()
    {
        SaveStoryBoardAfterUserClick();
    }

    /// <summary>
    /// This function is called when the Autosave function executes
    /// </summary> 
    public void OnAutoSaveClick()
    {
        SaveStoryBoardWithAutosave();
    }

    /// <summary>
    /// A button in the Storyboard Menu calls this function. A chosen file will be deleted permanent from storage
    /// To have a persistent data structure, the panel numbers will be recalculated after the deletion process
    /// </summary>
    public void OnDeleteButtonClick()
    {
        DeleteStoryboard();
    }

    #endregion

    #region Load_Storyboard

    /// <summary>
    /// This function is called by the onLoadButtonClick() function
    /// </summary>
    private void LoadStoryboardAfterClick()
    {
        /* 
        * Delete all existing Instances from the Instance_Container
        */

        string fileName = "";
        string completeFileName = "";
        bool isAutoSave = false;

        if (StaticVariables.fileName.Contains(AUTOSAVE))
        {
            isAutoSave = true;
            if (StaticVariables.selectedAutosaveStoryboard != "")
            {
                string tempFileName = StaticVariables.selectedAutosaveStoryboard;
                fileName = tempFileName.Substring(tempFileName.Length - 10);
                completeFileName = StaticVariables.savedFilepath + fileName + "/" + fileName + ".xml";
                StaticVariables.selectedAutosaveStoryboard = "";
                StaticVariables.fileName = "";
                StaticVariables.activeStoryBoard = StaticVariables.selectedAutosaveStoryboard;
                GameObject.FindGameObjectWithTag("LabelStoryboard").GetComponent<Text>().text = "Storyboard: " + StaticVariables.selectedAutosaveName;
                StartCoroutine(StartLoadingProcess(fileName, completeFileName, 2f, isAutoSave));
            }
        }
        else
        {
            isAutoSave = false;
            fileName = StaticVariables.fileName;
            completeFileName = StaticVariables.savedFilepath + fileName + "/" + fileName + ".xml";
            if (File.Exists(completeFileName) && fileName != "")
            {
                StartCoroutine(StartLoadingProcess(fileName, completeFileName, 2f, isAutoSave));
            }
            else
            {
                holoMessage.GetComponent<HoloMessage>().setMessage("Datei konnte nicht gefunden werden.", 3);
            }
        }
    }

    /// <summary>
    /// Starts the loading process and calls the function 'LoadStoryBoard'
    /// After the Storyboard has been loaded, the related Props for each panel are also loaded by calling the function 'LoadProps'
    /// </summary>
    /// <param name="fileName">The name of the file to load</param>
    /// <param name="cFileName">The path to the file</param>
    /// <param name="waittime">The time to wait</param>
    /// <param name="isAutoSave">Is the file to load an Autosave file of a normal user generated file</param>
    /// </summary> 
    private IEnumerator StartLoadingProcess(string fileName, string cFileName, float waittime, bool isAutoSave)
    {
        /*
         * Cleanup
         */

        panelListCanvas.GetComponent<PanelManager>().DestroyAllPanels();
        panelListCanvas.GetComponent<PanelManager>().DestroyPropsInSpawnContainer();

        /*
         * Store position of menu and send it 'far away' from the user to make it kind of invisible while the loading process is performed.
         */

        GameObject menus = GameObject.FindGameObjectWithTag("Menus");
        Vector3 oldPos = menus.transform.position;
        menus.transform.position = new Vector3(1000f, 1000f, 1000f);
        GameObject[] activePanels = GameObject.FindGameObjectsWithTag("Cell");
        holoMessage.GetComponent<HoloMessage>().setMessage("Bitte warten, Storyboard lädt.", waittime);

        /*
         * Wait until all Panels are destructed and function returns with true
         */

        yield return new WaitUntil(() => WaitForPanelDestruction(activePanels));

        /* 
         * Display wait message and stop until the XML file is loaded from file system
         * waittime = Time for the message to display, because main thread shows only a dark background while processing
         */

        yield return new WaitForSeconds(waittime);
        yield return new WaitUntil(() => LoadStoryboard(fileName, cFileName));

        holoMessage.GetComponent<HoloMessage>().setMessage("Storyboard wurde geladen.", waittime, new Color(0, 255, 0));

        /*
         * Set proper names to the Storyboard and 
         */

        if (isAutoSave)
        {
            StaticVariables.activeStoryBoard = StaticVariables.selectedAutosaveName;
        }
        else
        {
            StaticVariables.activeStoryBoard = fileName;
        }

        /*
         * Replace menu
         */

        menus.transform.position = oldPos;

        /*
         * Load the Props from file 
         */

        StartLoadingProps(isAutoSave);

        /*
         * End of loading process
         */

        /*
         * Initialize the Storyboard
         */

        GameObject.FindGameObjectWithTag("LabelStoryboard").GetComponent<Text>().text = "Storyboard: " + StaticVariables.activeStoryBoard;
        transform.Find(textEditFieldName).GetComponent<Text>().text = "";
        GameObject.FindGameObjectWithTag("AddPanelButton").GetComponent<AddButton>().SetNewPosition();
        GameObject.FindGameObjectWithTag("AddPanelButton").GetComponent<AddButton>().ResetBackgroundPanel();

        Resources.UnloadUnusedAssets();
        yield return true;
    }

    /// <summary>
    /// This class loads a storyboard from storage
    /// The storyboard can contain an image. During the loading process the image is read from storage and converted into a binary file. 
    /// This binary file is then part of the related panel. During the save-process, the binary file must not be converted in runtime and the save process is much faster.
    /// Using Threads is not possible for the conversion process, it has to happen on the main thread. Converting one ore more pictures would cause a huge delay.
    /// <param name="fileName">The fileName is the given Name for the xml file.</param>
    /// <param name="completeFileName">The path to the file.</param>
    /// </summary>
    private bool LoadStoryboard(string fileName, string completeFileName)
    {
        /*
         * Start loading and deserialize file
         */

        XmlSerializer ser = new XmlSerializer(typeof(StoryboardDataList));
        StoryboardDataList deserializedGameObject;
        FileStream fs = new FileStream(completeFileName, FileMode.Open);
        deserializedGameObject = (StoryboardDataList)ser.Deserialize(fs);

        /*
         * End of loading and deserialization
         */

        cellClone.SetActive(true);

        /*
         * Create clones from Panel prefab
         */

        foreach (StoryboardData data in deserializedGameObject.storyboardDatas)
        {
            /*
             * Instantiation and activation of prefab
             */
            GameObject currentPanelTemp = Instantiate(cellClone, anchorForPanelInstances.transform);
            currentPanelTemp.transform.tag = "Cell";
            currentPanelTemp.SetActive(true);

            /*
             * Write loaded data to panels
             */

            Transform currentChild = currentPanelTemp.transform.Find("Panel Content");
            currentChild.transform.Find("PanelPicture").GetComponent<RawImage>().texture = defaultTexture2D;
            currentChild.transform.Find(noteFieldName).GetComponent<KeyboardInputField>().text = data.noticeText;
            currentChild.transform.Find("Settings").GetComponent<Dropdown>().value = data.einstellungIndex;
            currentChild.transform.Find("Transition").GetComponent<Dropdown>().value = data.transitionIndex;

            /*
             * Load Image from file, if "data.picturePath" has got an entry 
             */

            if (data.picturePath != null)
            {
                StartCoroutine(LoadTextureAndFillIt(data, currentChild));
            }
        }
        cellClone.SetActive(false);
        fs.Dispose();

        return true;
    }

    /// <summary>
    /// Prepares the loading of a jpeg from storage.
    /// Also creates a bytearray for faster saving later 
    /// <param name="data">All Storyboard date read from file before</param>
    /// <param name="currentChild">The actual storyboard panel</param>
    /// </summary>
    IEnumerator LoadTextureAndFillIt(StoryboardData data, Transform currentChild)
    {
        Texture2D loadTex;
        RawImage rawImage;

        /*
         * Call load function  
         */

        loadTex = LoadTextureFromFile(data.picturePath);

        /*
         *  Set fetched Texture2D as RawImage to Panel child
         */

        currentChild.transform.GetChild(0).GetComponent<RawImage>().texture = loadTex;
        rawImage = currentChild.transform.Find("PanelPicture").GetComponent<RawImage>();
        rawImage.texture = LoadTextureFromFile(data.picturePath);
        rawImage.uvRect = new Rect(newX, newY, newWidth, newHeight);

        /*
         * Use the Texture2D to create a byte array 
         */

        StartCoroutine(SetByteArray(currentChild, loadTex));
        yield return true;
    }

    /// <summary>
    /// Loads a binary file, stores the value in a Texture2D and returns the Texture to the 'LoadTextureAndFillIt' function
    /// </summary>
    /// <param name="picturePath">The texture, which will be converted into a byte array</param>
    Texture2D LoadTextureFromFile(string picturePath)
    {
        /*
         * Returns a Texture 2D after loading it from file
         */

        byte[] bytes;
        bytes = File.ReadAllBytes(picturePath);
        Texture2D loadTexture = new Texture2D(1, 1);
        loadTexture.LoadImage(bytes);
        return loadTexture;
    }

    #endregion

    #region Save_Storyboard

    /// <summary>
    /// Saves the complete storyboard to storage.
    /// Calls several functions, to split the save process in smaller parts
    /// </summary>
    public void SaveStoryBoardAfterUserClick()
    {
        transform.Find(textEditFieldName).GetComponent<Text>().text = "TEST";
        if (savingIsActive == false)
        {
            savingIsActive = true;
            string newFileName = "";
            bool calledFromAutosave = false;
            
            {
                /*
                 * Is a button selected?
                 */

                if (StaticVariables.fileName != "")
                {
                    newFileName = StaticVariables.fileName;
                }

                /*
                 * Is the Editfield filled?
                 */

                else if (transform.Find(textEditFieldName).GetComponent<Text>().text != "")
                {
                    newFileName = transform.Find(textEditFieldName).GetComponent<Text>().text;
                }

                /*
                 * Is there an active Storyboard?
                 */

                else if (StaticVariables.activeStoryBoard != "")
                {
                    newFileName = StaticVariables.activeStoryBoard;
                }               
                else
                {
                    newFileName = UNNAMED;
                }
                char[] replacement = { ' ', ',', '.', ';', ':', '#', '/', '+', '*', '~', '-', '_', '!', '"', '§', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\' };
                for (int c = 0; c <= replacement.Length - 1; c++)
                {
                    newFileName = newFileName.Replace("" + replacement[c], "");
                }
            }

            GameObject[] storyBoardBackground = GameObject.FindGameObjectsWithTag("StoryBoardBackground");
            StoryboardDataList storyboardDataList = new StoryboardDataList();
            string storyboardPath = newFileName + "/";

            /*
             * Save process starts
             */

            DeleteAllFiles(storyboardPath);
            StartCoroutine(SaveAllFiles(storyBoardBackground, storyboardDataList, storyboardPath, newFileName, calledFromAutosave));

            /*
             * Save process end
             */

            UpdateAllFiles();
            ResetFileName("");
            savingIsActive = false;
        }
    }

    /// <summary>
    /// In case of Autosave, the behave of the saving process is a little different to the way it is by buttonclick
    /// </summary>
    public void SaveStoryBoardWithAutosave()
    {
        if (savingIsActive == false)
        {
            savingIsActive = true;
            string newFileName = "";
            string storyboardPath = "";
            bool calledFromAutosave = true;

            GameObject[] storyBoardBackground = GameObject.FindGameObjectsWithTag("StoryBoardBackground");
            StoryboardDataList storyboardDataList = new StoryboardDataList();

            /*
             * The backup counter adds a number to the Autosave file. This is like a mini versioning functionality.
             * The crucial part of the Autosave backup restore process is the timestamp of the saved file, and not the number.
             */

            backupCounter++;
            if (backupCounter == 4)
            {
                backupCounter = 1;              
            }
            newFileName = AUTOSAVE + "_" + backupCounter;
            storyboardPath = newFileName + "/";
            

            /*
             *  Save process starts
             */

            DeleteAllFiles(storyboardPath);
            StartCoroutine(SaveAllFiles(storyBoardBackground, storyboardDataList, storyboardPath, newFileName, calledFromAutosave));

            savingIsActive = false;
        }
    }

    /// <summary>
    /// Save all collected Data to file
    /// </summary>
    /// <param name="storyBoardBackground">The Father Node</param>
    /// <param name="storyboardDataList">The DataList with all Panels stored</param>
    /// <param name="storyboardPath">The Ppath to save</param>
    /// <param name="newFileName">The complete filename</param>
    /// <param name="isCalledFromAutosave">If the function called from autosave or by button click</param>
    private IEnumerator SaveAllFiles(GameObject[] storyBoardBackground, StoryboardDataList storyboardDataList, string storyboardPath, string newFileName, bool isCalledFromAutosave)
    {
        string onlyFileName = "";
        string folderName = "";

        /*
         * Saving:
         * - Storyboard with all existing panels
         * - Images
         * - Existing Props
         * - Flag file - if called from the Autosave function (isAutoSave)
         */

        if (!Directory.Exists(StaticVariables.savedFilepath + storyboardPath))
        {
            Directory.CreateDirectory(StaticVariables.savedFilepath + storyboardPath);
        }

        /*
         * Wait until all Files are saved to file, before the FlagFile will be created
         * This assures, that the file bundle is complete
         */

        if (isCalledFromAutosave)
        {
            onlyFileName = AUTOSAVE;
            folderName = newFileName;
        }
        else
        {
            onlyFileName = newFileName;
            folderName = newFileName;
        }
        StartCoroutine(StartSavingStoryboard(storyboardPath, newFileName, storyboardDataList, isCalledFromAutosave, storyBoardBackground));
        StartCoroutine(StartSavingProps(onlyFileName, folderName, isCalledFromAutosave));

        if (isCalledFromAutosave)
        {
            StartCoroutine(SaveFlagFileToAutosave(StaticVariables.savedFilepath + storyboardPath, StaticVariables.activeStoryBoard));
        }                 

        /*
         * The StaticVariables.activeStoryBoard contains the actual loaded File name
         */
        if (!isCalledFromAutosave)
        {
            StaticVariables.activeStoryBoard = newFileName;
            GameObject.FindGameObjectWithTag("LabelStoryboard").GetComponent<Text>().text = "Storyboard: " + newFileName;
            GameObject.FindGameObjectWithTag("StoryBoardNameParent").GetComponent<KeyboardInputField>().text = "";
        }

        // Debug.Log("SAVEALLFILES: Filename: " + newFileName + " | Active Storyboard: " + StaticVariables.activeStoryBoard + " | Autosave aktiv: " + isCalledFromAutosave);

        yield return true;
    }

    /// <summary>
    /// Creates a Storyboard and saves it to file system
    /// </summary>
    /// <param name="storyboardPath">The path for the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="storyboardDataList">The Class for Storing the DataList.</param>
    /// <param name="isAutoSave">If the function called from Autosave or from button.</param>
    /// <param name="storyBoardBackground">The Father Node as an anchor.</param>
    private IEnumerator StartSavingStoryboard(string storyboardPath, string fileName, StoryboardDataList storyboardDataList, bool isAutoSave, GameObject[] storyBoardBackground)
    {
        // Debug.Log("StartSavingStoryboard - Filename: " + fileName + " | Autosave aktiv: " + isAutoSave);

        /*
        * Create Storyboard elements
        */

        for (int index = 0; index < storyBoardBackground.Length; index++)
        {
            string pictureFilename = ("sbPic" + index);
            int dataSiblinIndex = index;
            Transform currentChild = storyBoardBackground[index].transform;
            string dataNotice = currentChild.Find(noteFieldName).GetComponent<Text>().text;
            int dataEinstellung = currentChild.Find("Settings").GetComponent<Dropdown>().value;
            int dataTransition = currentChild.Find("Transition").GetComponent<Dropdown>().value;

            /*
            * Converted JPG to improve performance while saving. Then save the Image
            */

            byte[] convertedTexture = new byte[currentChild.GetChild(0).GetComponent<JPGtoByte>().GetByteArraySize()];
            convertedTexture = currentChild.GetChild(0).GetComponent<JPGtoByte>().GetByteArray();
            SaveImageToFileSystem(StaticVariables.savedFilepath + storyboardPath + pictureFilename, convertedTexture);
            StoryboardData tempStoryBoardData = new StoryboardData(dataSiblinIndex, dataNotice, dataEinstellung, dataTransition, StaticVariables.savedFilepath + storyboardPath + pictureFilename);
            storyboardDataList.AddStoryboard(tempStoryBoardData);
            yield return true;
        }

        /*
         * Start of Serialisation
         */

        FileStream fs = new FileStream(StaticVariables.savedFilepath + storyboardPath + fileName + ".xml", FileMode.Create);
        TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
        XmlSerializer ser = new XmlSerializer(typeof(StoryboardDataList));
        ser.Serialize(writer, storyboardDataList);
        storyboardDataList = null;
        writer.Dispose();
        fs.Dispose();

        /*
         * End of Serialisation
         */

        storyBoardSaved = true;
        yield return true;
    }

    /// <summary>
    /// Image file is saved here
    /// </summary>
    /// <param name="filePath">The path for the file.</param>
    private bool SaveImageToFileSystem(string filePathWithName, byte[] convertedTexture)
    {
        File.WriteAllBytes(filePathWithName, convertedTexture);
        return true;
    }

    /// <summary>
    /// Flag file, to check integrity of saved files. 
    /// Only if this file is part of the save bundle, the autosave will be accepted.
    /// </summary>
    /// <param name="filePath">The path for the file.</param>
    private IEnumerator SaveFlagFileToAutosave(string filePath, string fileName)
    {
        yield return new WaitUntil(() => storyBoardSaved);
        yield return new WaitUntil(() => propsSaved);
        try
        {
            File.WriteAllText(filePath + fileName + FLAGFILESUFFIX, "Bundle complete");
        }
        catch(Exception e)
        {
            Debug.Log("Could not save Flag File: " + e);
        }
        storyBoardSaved = false;
        propsSaved = false;
        yield return null;
    }

    #endregion

    #region Delete_Storyboard

    /// <summary>
    /// Deletes the complete storyboard from storage
    /// </summary>
    public void DeleteStoryboard()
    {
        string fileName = "";

        if (StaticVariables.fileName != "")
        {
            fileName = StaticVariables.fileName;
        }

        if (fileName == null || fileName == "")
        {
            holoMessage.GetComponent<HoloMessage>().setMessage(StaticVariables.saveloadStoryboard_infoText3, 3f);
        }
        else
        {
            if (Directory.Exists(StaticVariables.savedFilepath + fileName))
            {
                /*
                 * Delete complete folder and all content in it
                 */

                Directory.Delete(StaticVariables.savedFilepath + fileName, true);
                holoMessage.GetComponent<HoloMessage>().setMessage(StaticVariables.saveloadStoryboard_infoText4 + "'" + fileName + "'" + StaticVariables.saveloadStoryboard_infoText5, 3f);

                /*
                 * Update the UI, so that the deleted file vanishes
                 */

                UpdateAllFiles();
            }
            else
            {
                holoMessage.GetComponent<HoloMessage>().setMessage(StaticVariables.saveloadStoryboard_infoText4 + "'" + fileName + "'" + StaticVariables.saveloadStoryboard_infoText6, 3f);
            }
        }
        ResetFileName("");
    }

    #endregion

    #region Props Saving

    /// <summary>
    /// This class serializing the object ObjectDataLists with XmlSerializer
    /// </summary>
    /// <param name="storyboardName">The fileName is the given Name for the xml file.</param>
    private IEnumerator StartSavingProps(string onlyFileName, string folderName, bool isAutoSave)
    {
        string completeStoryboardName = StaticVariables.savedFilepath + folderName + "/";

        // Debug.Log("Saving Props to: " + completeStoryboardName + " | Autosave aktiv: " + isAutoSave);
        GameObject[] allActivePropsContainer = GameObject.FindGameObjectsWithTag("PropsContainer");
        for (int cellNumber = 0; cellNumber < allActivePropsContainer.Length; cellNumber++)
        {
            ObjectDataList objectDataList = new ObjectDataList();
            string nameOfPropsContainer = allActivePropsContainer[cellNumber].name;
            string propsContainerNumberString = nameOfPropsContainer.Substring(20);
            string filename = onlyFileName + (propsContainerNumberString + "SC" + ".xml");
            GameObject tempPropContainer = GameObject.Find("_Instance.Container_" + propsContainerNumberString) as GameObject;
            int propsContainerSize = tempPropContainer.transform.childCount;
            if (propsContainerSize > 0)
            {
                FileStream fs = new FileStream(completeStoryboardName + filename, FileMode.Create);
                TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
                XmlSerializer ser = new XmlSerializer(typeof(ObjectDataList));
                for (int childs = 0; childs < propsContainerSize; childs++)
                {
                    GameObject newGameObject;
                    newGameObject = tempPropContainer.transform.GetChild(childs).gameObject;
                    string dataObjectName = newGameObject.name;
                    string dataTag = newGameObject.tag;
                    float dataPositionX = newGameObject.transform.localPosition.x;
                    float dataPositionY = newGameObject.transform.localPosition.y;
                    float dataPositionZ = newGameObject.transform.localPosition.z;
                    float dataRotationX = newGameObject.transform.localEulerAngles.x;
                    float dataRotationY = newGameObject.transform.localEulerAngles.y;
                    float dataRotationZ = newGameObject.transform.localEulerAngles.z;
                    float dataScaleX = newGameObject.transform.localScale.x;
                    float dataScaleY = newGameObject.transform.localScale.y;
                    float dataScaleZ = newGameObject.transform.localScale.z;
                    bool hasHead;
                    float dataChildPositionX = 0;
                    float dataChildPositionY = 0;
                    float dataChildPositionZ = 0;
                    float dataChildRotationX = 0;
                    float dataChildRotationY = 0;
                    float dataChildRotationZ = 0;
                    float dataChildScaleX = 0;
                    float dataChildScaleY = 0;
                    float dataChildScaleZ = 0;
                    string dataNoticeText = "";

                    if (newGameObject.transform.Find("Head"))
                    {
                        Transform headTransform = newGameObject.transform.Find("Head");
                        hasHead = true;
                        dataChildPositionX = headTransform.localPosition.x;
                        dataChildPositionY = headTransform.localPosition.y;
                        dataChildPositionZ = headTransform.localPosition.z;
                        dataChildRotationX = headTransform.localEulerAngles.x;
                        dataChildRotationY = headTransform.localEulerAngles.y;
                        dataChildRotationZ = headTransform.localEulerAngles.z;
                        dataChildScaleX = headTransform.localScale.x;
                        dataChildScaleY = headTransform.localScale.y;
                        dataChildScaleZ = headTransform.localScale.z;
                        dataNoticeText = newGameObject.transform.Find("ButtonsWithChild(Clone)/NoticeboardCanvas/NoticeboardBackground/NoticeboardText").GetComponent<Text>().text;
                    }
                    else
                    {
                        hasHead = false;
                        dataNoticeText = newGameObject.transform.Find("Buttons(Clone)/NoticeboardCanvas/NoticeboardBackground/NoticeboardText").GetComponent<Text>().text;
                    }
                    objectDataList.AddGameObject(new ObjectData(dataObjectName, dataTag, dataPositionX, dataPositionY, dataPositionZ, dataRotationX, dataRotationY, dataRotationZ, dataScaleX, dataScaleY, dataScaleZ,
                            hasHead, dataChildPositionX, dataChildPositionY, dataChildPositionZ, dataChildRotationX, dataChildRotationY, dataChildRotationZ, dataChildScaleX, dataChildScaleY, dataChildScaleZ,
                            dataNoticeText));
                    /*
                    Debug.Log("saved position: " + dataScaleX + ", " + dataScaleY + ", " + dataScaleZ);
                    Debug.Log("Save true. Objectname: " + dataObjectName + ", Tag: " + dataTag +
                               ", Pos(x,y,z): " + dataPositionX + ", " + dataPositionY + ", " + dataPositionZ +
                               ", Rot(x,y,z): " + dataRotationX + ", " + dataRotationY + ", " + dataRotationZ +
                               ", Pos(x,y,z): " + dataScaleX + ", " + dataScaleY + ", " + dataScaleZ);
                               */
                }

                ser.Serialize(writer, objectDataList);
                objectDataList = null;
                writer.Dispose();
                fs.Dispose();
            }
            if (!completeStoryboardName.Contains(AUTOSAVE))
            {
                holoMessage.GetComponent<HoloMessage>().setMessage(StaticVariables.saveloadStoryboard_infoText4 + StaticVariables.saveloadStoryboard_infoText2, 3);
            }
        }
        propsSaved = true;
        yield return true;
    }


    #endregion

    #region Props Loading and Deserialization

    /// <summary>
    /// The method is called if the button 'Laden' is pressed and
    /// calls the method ReadObject(filename)
    /// </summary>
    public void StartLoadingProps(bool isAutoSave)
    {
        
        string storyboardNameString = StaticVariables.activeStoryBoard;
        if (isAutoSave)
        {
            storyboardNameString = StaticVariables.selectedAutosaveName;
        }
        string path = StaticVariables.savedFilepath + storyboardNameString + "/";
        GameObject[] allActivePanels = GameObject.FindGameObjectsWithTag("Cell");
        for (int cellNumber = 1; cellNumber <= allActivePanels.Length; cellNumber++)
        {
            string filename = storyboardNameString + cellNumber.ToString() + "SC" + ".xml";
            try
            {
                if (File.Exists(path + filename))
                {                  
                    LoadProps(path, filename, cellNumber);
                }              
            }
            catch (SerializationException serExc)
            {
                Debug.Log("Serialization Failed");
                Debug.Log(serExc.Message);
            }
        }
    }

    /// <summary>
    /// This method deserializes the ObjectDataList and
    /// loads all the objects into the scene with their saved values.
    /// </summary>
    /// <param name="fileName">The fileName is the given name for the XML file.</param>
    /// <param name="path">Directory where the file will be read from.</param>
    /// <param name="cellNumber">Cell where the Props will be added.</param>
    private void LoadProps(string path, string fileName, int cellNumber)
    {
        bool isPropLoadedFromFile = true;
        XmlSerializer ser = new XmlSerializer(typeof(ObjectDataList));
        ObjectDataList deserializedGameObject;
        FileStream fs = new FileStream(path + fileName, FileMode.Open);
        deserializedGameObject = (ObjectDataList)ser.Deserialize(fs);
        foreach (ObjectData data in deserializedGameObject.objectDatas)
        {
            if (data.hasHead)
            {
                var spawnerScript = GameObject.Find("SpawnPoints").GetComponent<PropsFactory>();
                Vector3 position = new Vector3(data.positionX, data.positionY, data.positionZ);
                Vector3 rotation = new Vector3(data.rotationX, data.rotationY, data.rotationZ);
                Vector3 positionChild = new Vector3(data.childPositionX, data.childPositionY, data.childPositionZ);
                Vector3 rotationChild = new Vector3(data.childRotationX, data.childRotationY, data.childRotationZ);
                string originalNamePart = "";
                if (data.objectName.Contains("_PanelTech"))
                {
                    data.objectName = data.objectName.Remove(data.objectName.Length - 17);
                    originalNamePart = "_PanelTech";
                }
                if (data.objectName.Contains("_PanelRequisite"))
                {
                    data.objectName = data.objectName.Remove(data.objectName.Length - 22);
                    originalNamePart = "_PanelRequisite";
                }
                try
                {
                    spawnerScript.SpawnObject(Resources.Load<GameObject>("Prefabs/Tech/" + data.objectName), position, rotation, positionChild, rotationChild, data.noteText, isPropLoadedFromFile, cellNumber, originalNamePart);
                }
                catch (System.Exception)
                {
                    try
                    {
                        spawnerScript.SpawnObject(Resources.Load<GameObject>("Prefabs/Requisite/" + data.objectName), position, rotation, positionChild, rotationChild, data.noteText, isPropLoadedFromFile, cellNumber, originalNamePart);
                    }
                    catch (System.Exception)
                    {

                    }
                }
            }
            else
            {
                var spawnerScript = GameObject.Find("SpawnPoints").GetComponent<PropsFactory>();
                Vector3 position = new Vector3(data.positionX, data.positionY, data.positionZ);
                Vector3 rotation = new Vector3(data.rotationX, data.rotationY, data.rotationZ);
                string originalNamePart = "";
                if (data.objectName.Contains("_PanelTech"))
                {
                    data.objectName = data.objectName.Remove(data.objectName.Length - 17);
                    originalNamePart = "_PanelTech";
                }
                if (data.objectName.Contains("_PanelRequisite"))
                {
                    data.objectName = data.objectName.Remove(data.objectName.Length - 22);
                    originalNamePart = "_PanelRequisite";
                }
                try
                {
                    spawnerScript.SpawnObject(Resources.Load<GameObject>("Prefabs/Tech/" + data.objectName), position, rotation, data.noteText, isPropLoadedFromFile, cellNumber, originalNamePart);
                }
                catch (System.Exception)
                {
                    try
                    {
                        spawnerScript.SpawnObject(Resources.Load<GameObject>("Prefabs/Requisite/" + data.objectName), position, rotation, data.noteText, isPropLoadedFromFile, cellNumber, originalNamePart);
                    }
                    catch (System.Exception)
                    {

                    }
                }
            }
        }
        fs.Dispose();
        ResetFileName("");
    }

    #endregion
}

#region Serialization

/// <summary>
/// This class defines has a list to add multiple Storyboard.
/// The Storyboard is from the class StoryboardData.
/// </summary>
[XmlRoot("StoryboardDataLists")]
[XmlInclude(typeof(StoryboardData))]
public class StoryboardDataList
{
    [XmlArray("StoryboardDatasArray")]
    [XmlArrayItem("StoryboardData")]
    public List<StoryboardData> storyboardDatas = new List<StoryboardData>();

    [XmlElement("Listname")]
    public string Listname { get; set; }

    public StoryboardDataList() { }

    public StoryboardDataList(string name)
    {
        this.Listname = name;
    }

    public void AddStoryboard(StoryboardData StoryboardData)
    {
        storyboardDatas.Add(StoryboardData);
    }
}

/// <summary>
/// StoryboardData defines several attributes,
/// to serialize/deserialize the gameStoryboard.
/// </summary>
[XmlType("StoryboardData")]
public class StoryboardData
{
    [XmlElement(Namespace = "Sibling Index")]
    public int siblingIndex;
    [XmlElement(Namespace = "Notice Text")]
    public string noticeText;
    [XmlElement(Namespace = "Einstellung Index")]
    public int einstellungIndex;
    [XmlElement(Namespace = "Uebergang Index")]
    public int transitionIndex;
    [XmlElement(Namespace = "Path to the Picture")]
    public string picturePath;

    public StoryboardData(int siblingIndex, string noticeText, int einstellungIndex, int transitionIndex, string picturePath)
    {
        this.siblingIndex = siblingIndex;
        this.noticeText = noticeText;
        this.einstellungIndex = einstellungIndex;
        this.transitionIndex = transitionIndex;
        this.picturePath = picturePath;
    }
    public StoryboardData()
    {

    }
}

#endregion

