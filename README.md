# Spectrum
Spectrum is a free and open source color gradient game. Puzzle boards are generated at random using voronoi diagrams. <br>

[Getting Started](#getting-started)<br>
[Downloads](#downloads)<br>
[Screenshots](#screenshots)<br>
[Credits](#credits) <br>
[About The Creator](#about-the-creator) <br>
[License](#license) <br>

<img src="https://user-images.githubusercontent.com/52141232/72937161-6f5e0500-3d1d-11ea-95fc-b621ccc0668e.png" alt="Spectrum Opening Window" width = 200 display="inline"> <img src="https://user-images.githubusercontent.com/52141232/72937563-38d4ba00-3d1e-11ea-9c0a-8a0c592462f3.png" alt="Spectrum Color Picker" width = 200 display="inline"> <img src="https://user-images.githubusercontent.com/52141232/72937524-28bcda80-3d1e-11ea-9428-e66f5dbaed9d.png" alt="Spectrum Game Window 2" width = 200 display="inline">

## Getting Started
1. [Download](#downloads) require software and source code 
2. [Setup](#setup) in Unity 
3. (Optional) [Build and Run in XCode](#(optional)-build-and-run)

### Downloads
**Requirements**
- <a href="https://unity.com/" target="_blank">Unity Game Engine</a> version 2018.4.14f1
- Computer
- (Optional) <a href="https://developer.apple.com/xcode/" target="_blank">Apple XCode</a> version 11.3.1
- (Optional) iPhone SE or iPhone SE Simulator

**Download Source Code** <br>
Use one of the following methods to download Spectrum source code:
- [Download source code to Finder](https://minhaskamal.github.io/DownGit/#/home?url=https://github.com/rvesteinsdottir/spectrum)
- Clone the repository locally: ```  git clone https://github.com/rvesteinsdottir/spectrum.git  ```

### Setup
Open repository in Unity:
1. Open Unity and select Open, navigate to downloaded spectrum-master folder and open in Unity
2. Once project is opened, select File -> Build Settings. In Build Settings select iOS and click "Switch Platform"

### (Optional) Build and Run 
Set Player Settings in Unity:
1. Player Settings -> Other Settings change Target Device to:  
    - Device SDK for iPhone build
    - **OR** Simulator SDK for iPhone Simulator build
2. Player Settings -> Other Settings confirm Identification Bundle Identifier is "game.unity2d.Spectrum"
3. Once Player Settings are confirmed, navigate to File -> Build Settings -> Build
4. After build is complete, open build folder. Open file "Unity-iPhone.xcodeproj" in XCode

Set up Xcode to run the project:
1. XCode -> Preferences -> Accounts, select or add your Apple ID
2. In menu on the top left, click folder icon to show Project Navigator
3. Double click on Unity-iPhone
4. Select Signing & Capabilities
5. Check box next to "Automatically manage signing"
6. Add your Apple ID to the "Team" using dropdown

Run on device via XCode:
1. Connect iPhone SE to computer
2. In top toolbar make sure your device is listed next to "Unity-iPhone"
3. Wait for indexing to complete
4. Select play to run 

**OR** run on a iOS Simulator via XCode:
1. Select Window -> Devices and Simulators
2. If iPhone SE is not on list, click "+" to add iPhone SE
3. In top toolbar make sure iPhone SE simulator is listed next to "Unity-iPhone"
4. Wait for indexing to complete
5. Select play to run 

## Screenshots
<img src="https://user-images.githubusercontent.com/52141232/72937161-6f5e0500-3d1d-11ea-95fc-b621ccc0668e.png" alt="Spectrum Opening Window" width = 200 display="inline"> <img src="https://user-images.githubusercontent.com/52141232/72937563-38d4ba00-3d1e-11ea-9c0a-8a0c592462f3.png" alt="Spectrum Color Picker" width = 200 display="inline"> <br>
<img src="https://user-images.githubusercontent.com/52141232/72937483-104cc000-3d1e-11ea-9b80-d679561a0f04.png" alt="Spectrum Game Window 1" width = 200 display="inline"> <img src="https://user-images.githubusercontent.com/52141232/72937524-28bcda80-3d1e-11ea-9428-e66f5dbaed9d.png" alt="Spectrum Game Window 2" width = 200 display="inline"> <img src="https://user-images.githubusercontent.com/52141232/72937556-35d9c980-3d1e-11ea-831e-1ca867ddd3cf.png" alt="Spectrum Game Window 3" width = 200 display="inline">


## Credits
Audio/Graphics: <a href="https://kenney.nl/" target="_blank">Kenney Free Game Assets</a> <br>
Logo: <a href="https://logomakr.com/" target="_blank">LogoMakr.com</a> <br>
Triangulator: <a href="http://wiki.unity3d.com/index.php?title=Triangulator" target="_blank">Unify Community Triangulator</a> <br>
Voronoi Diagram Library: <a href="https://github.com/PouletFrit/csDelaunay" target="_blank">C# Delaunay Library</a> <br>

## About the Creator
[Portfolio](https://rvesteinsdottir.github.io/), Raisah Vesteinsdottir

## License
[MIT License](https://github.com/rvesteinsdottir/spectrum/blob/master/LICENSE), Raisah Vesteinsdottir

