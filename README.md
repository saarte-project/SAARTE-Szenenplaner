Repository for AR-Szenenplaner-App in SAARTE project
====================================================

The AR-Szenenplaner-App has been developed using the unity game engine (https://unity.com/) for the HoloLens 1 device (https://docs.microsoft.com/en-us/hololens/hololens1-hardware).

Contents
--------

1. Project Directory Structure
2. Software Requirements
3. Setup
4. Building
5. Acknowledgements
  
Project Directory Structure
---------------------------

* **3D-Models** Unchanged models used in project but independent from Unity
* **App** Used for building the Szenenplaner app
* **Assets** Most Unity-related stuff of the project
* **Assets/Scripts** C# code of the Unity project
* **Doc** Documentation for users (handbook) and developers (API)
* **Library** Created by Unity (might not be existen before build)
* **License** Lincens information regarding third party assets
* **ProjectSettings** Created by Unity
* **Temp** Temporarily created by Unity (might not be existen before build)


Software requirements
---------------------

* Unity-Version: 2017.4.28f1 (https://unity3d.com/unity/qa/lts-releases)
	* Activate all Windows components in installer.

* Windows 10 needs to have the Windows 10 SDK installed. This update can be found here:
	* https://developer.microsoft.com/en-us/windows/downloads/windows-10-sdk
	* After the installation the dropdown "SDK" (in build settings) should automatically be set to "Latest Version" by Unity. 

* HoloToolkit 2017.4.3.0 (https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)
	* HoloToolkit (Unity Package)
	* HoloToolkit-Example (Unity Package)

* Documentation
	* Doxygen 1.8

* 3D Model conversion
	* Blender (https://www.blender.org/)
	* Blender needs to be installed (for enabling Unity to import models)


Setup
-----

1. Install above mentioned software requirements.
2. Optional steps
	1. Add the directory of the git application to your environment variable PATH
3. Before the application is first tested in Unity's play mode, the build process has to be started at least once to enable some HoloLens specific functionality.

Building
--------

1. Build Project in Unity (into directory "App")
	1. Use the following settings
		-  Target Device: HoloLens
		-  Build Type: D3D
		-  SDK: Latest installed
		-  Visual Studio Version: Latest installed
		-  Build and Run on: Local machine
		-  Do **not** use "Unity C# Projects" in when using Unity 2017.3.1f1
	2. Build.
2. Build Project in Visual Studio
	- Open "SAARTE-Szenenplaner.sln" in "App" directory
	- Set Configuration to "Master" and "x86"
	- Start building for example "Remote Machine"
    
Acknowledgements
----------------
This work has been performed in project SAARTE (Spatially-Aware Augmented Real-
ity in Teaching and Education). SAARTE is supported by the European Union (EU)
in the ERDF program P1-SZ2-7 and by the German federal state Rhineland-Palatinate
(Antr.-Nr. 84002945).


