Hints for Developers
====================

* Merging with UnityYAMLMerge 
	* https://docs.unity3d.com/Manual/SmartMerge.html
    * For **sourcetree** see above link.
	* For **git in command line** adapt .gitconfig as follows 
		* E.g.  
			[merge]  
				tool = unityyamlmerge  
			  
			[mergetool "unityyamlmerge"]  
				trustExitCode = false  
				cmd = 'C:\\Program Files\\Unity\\Editor\\Data\\Tools\\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"  
	* For **sourcetree** and **git in command line** adapt mergespecfile.txt as follows
		* E.g. with kdiff3  
		      unity use "%programs%\KDiff3\kdiff3.exe" "%b" "%l" "%r" -o "%d"  
		      prefab use "%programs%\KDiff3\kdiff3.exe" "%b" "%l" "%r" -o "%d"  
		      * use "%programs%\KDiff3\kdiff3.exe" "%b" "%l" "%r" -o "%d"  



* Developers might be interested in the following links
  * https://unity.com/developers-corner
  * https://unity.com/creators-corner
  * https://www.toptal.com/unity-unity3d/top-unity-development-mistakes
