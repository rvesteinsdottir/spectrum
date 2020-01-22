# Spectrum
A soothing color gradient game using random voronoi diagram generation.
Developed and tested on Unity 2018.4.14f1

[Getting Started](#getting-started)<br>
[Downloads](#downloads)<br>
[Screenshots](#screenshots)<br>
[Credits](#credits)

photo photo

## Getting Started
1. [Download](#downloads) require software and source code 
2. [Setup](#setup) in Unity 
3. (Optional) [Build and Run in XCode](#(optional)-build-and-run)

### Downloads
**Requirements**
- [Unity Game Engine](https://unity.com/) version 2018.4.14f1
- Computer
- (Optional) [Apple XCode](https://developer.apple.com/xcode/) version 11.3.1
- (Optional) iPhone SE or iPhone SE Simulator

**Download Source Code** <br>
Use one of the following methods to download Spectrum source code:
- [Download source code to Finder](https://github.com/rvesteinsdottir/Spectrum#download)
- Clone the repository locally: 
```
git clone https://github.com/rvesteinsdottir/spectrum.git
```

### Setup
Open repository in Unity:
1. Open Unity
2. Select Open and navigate to downloaded spectrum-master folder and open in Unity
3. Once project is opened, select File -> Build Settings
4. Select iOS and select "Switch Platform"

### (Optional) Build and Run 
Set Player Settings in Unity:
1. Player Settings -> Other Settings change Target Device to:  
    - Device SDK for iPhone build
    - **OR** Simulator SDK for iPhone Simulator build
2. Player Settings -> Other Settings confirm Identification Bundle Identifier is "game.unity2d.Spectrum"
3. Select File -> Build Settings -> Build
4. Once build is complete, open build folder, open file "Unity-iPhone.xcodeproj" in XCode

Set up Xcode to run the project:
1. XCode -> Preferences -> Accounts. Select or add your Apple ID
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

## Credits
Graphics: [Kenney Free Game Assets](https://kenney.nl/) <br>
Logo: [LogoMakr.com](https://logomakr.com/) <br>
Triangulator: [Unify Community Triangulator](http://wiki.unity3d.com/index.php?title=Triangulator) <br>
Voronoi Diagram Library: [C# Delaunay Library](https://github.com/PouletFrit/csDelaunay)

## License

Copyright (c) 2020, [Raisah Vesteinsdottir](https://github.com/rvesteinsdottir/Spectrum/LICENSE)

