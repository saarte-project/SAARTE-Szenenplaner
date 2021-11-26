#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Build;
using UnityEditor;
using System.Diagnostics;
using System;


/// <summary>
/// Automatically generates and stores the build time in a file.
/// </summary>
[ExecuteInEditMode]
public class BuildTimeScript : MonoBehaviour, IPreprocessBuild
{
    /// <summary>
    /// Before build starts the time is taken and stored in a file.
    /// </summary>
    /// <param name="target">Unused parameter</param>
    /// <param name="path">Unused parameter</param>
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        string buildTime = "empty";
        buildTime = generateBuildInfo();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        checkIfAssetCorrectlyUpdated(buildTime);
    }

    public int callbackOrder { get { return 0; } }


    /// <summary>
    /// Time is taken and stored in a file.
    /// </summary>
    /// <returns>The build info.</returns>
    private string generateBuildInfo()
    {
        string buildInfo = "";
        buildInfo += System.DateTime.Now.ToString("u");
		try
		{
			buildInfo += " (branch: " + getGitBranch() + ")";
			buildInfo += " [commit: " + getGitHash() + "]";
            if (uncommitedChangesExist())
                buildInfo += " ----> Uncommited changes exist!";
		}
		catch( Exception e )
		{
			UnityEngine.Debug.Log( "Problem using git. Exception caught: (" + e.Message + ")" );
		}

        const string fullFileName = "Assets/Resources/" + BuildTimeDisplay.fileName + ".txt";
        string[] lines = { buildInfo };
        System.IO.File.WriteAllLines(@"" + fullFileName, lines);
        UnityEngine.Debug.Log("Wrote build info to file " + buildInfo);
        
        return buildInfo;
    }

    private string getGitBranch()
    {
        const string gitCommand = "git";
        const string gitArgument = @" rev-parse --abbrev-ref HEAD";
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.Arguments = gitArgument;
        p.StartInfo.FileName = gitCommand;
        p.Start();

        System.IO.StreamReader sOut = p.StandardOutput;

        string result = sOut.ReadToEnd();

        result = System.Text.RegularExpressions.Regex.Replace(result, @"\n", "");

        UnityEngine.Debug.Log("Buiding branch: " + result);
        return result;
    }

    private string getGitHash()
    {
        const string gitCommand = "git";
        const string gitArgument = @"  rev-parse --short HEAD";
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.Arguments = gitArgument;
        p.StartInfo.FileName = gitCommand;
        p.Start();

        System.IO.StreamReader sOut = p.StandardOutput;

        string result = sOut.ReadToEnd();

        result = System.Text.RegularExpressions.Regex.Replace(result, @"\n", "");

        UnityEngine.Debug.Log("Building commit: " + result);
        return result;
    }

    private bool uncommitedChangesExist()
    {
        const string gitCommand = "git";
        const string gitArgument = @"   diff-index --quiet HEAD --";
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.Arguments = gitArgument;
        p.StartInfo.FileName = gitCommand;
        p.Start();

        p.WaitForExit();
        int result = p.ExitCode;

        return (result == 1);
    }

    /// <summary>
    /// Sanity check if the build info in the files is equal to the generated info.
    /// </summary>
    /// <param name="buildInfo">Generated build info.</param>
    private void checkIfAssetCorrectlyUpdated(string buildInfo)
     {
        string buildInfoFromFile = ((TextAsset)Resources.Load(BuildTimeDisplay.fileName, typeof(TextAsset))).text;

        // remove newlines
        buildInfoFromFile = System.Text.RegularExpressions.Regex.Replace(buildInfoFromFile, @"\t|\n|\r", "");

        UnityEngine.Debug.Log("Read build info from file " + buildInfoFromFile);
        if (buildInfo != buildInfoFromFile)
        {
            throw new System.ArgumentException("Written info does not equal info re-read from file.", "original");
        }        
    }
}
#endif // UNITY_EDITOR