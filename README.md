# GeospatialCreatorDemos
Demos with Geospatial Creator With Unity AR Foundation and ARCore Extensions.

📢 Be sure to watch my 🎥 [full YouTube video](
https://youtu.be/f3kf16TVWMo) explaining all the steps to configure Google's Geospatial Creator With Unity.

### 📚 Demo Scenes (More coming)
|Scene: Geospatial.unity||
|---|---|
|**Robot** In Salt Lake City|**Robot** In Paris|
|<img src="https://github.com/dilmerv/GeospatialCreatorDemos/blob/master/docs/images/GeospatialRobot_1.gif" width="280">|<img src="https://github.com/dilmerv/GeospatialCreatorDemos/blob/master/docs/images/GeospatialRobot_2.gif" width="280">|

### iOS & Android Build Settings

- Configure Android Project:
  - Switch to Android
  - Change to Gamma
  - Graphics API > Disable / Remove Vulkan
  - Minimum API = Version 28
  - Scripting Backend = IL2CPP
  - ARM64 Only
  - Under XR-Plugin Managemenr > Android > Enable ARCore Plugin or configure it
- Configure iOS Project
  - Switch to iOS
  - Add Camera Usage Description
  - Add Location Usage Description
  - Enable Requires ARKit Support

### iOS & Android Device Deployment
- Android
  - Simply connect to your computer via USB-C > Build And Run
- iOS
  - Simply connect to your computer via Lightninig to USB-C > Build And Run  
  - In XCode generated project apply these changes:
    - Search for BitCode under Build Settings > Set It To No
    - Add ARCore package to Unity-iPhone
    - Enable ARCore Geospatial component

💡 For a more detailed manual on Geospatial configuration take a look at [Google's ARCore documentation](https://developers.google.com/ar/geospatialcreator/intro)

ℹ️ Credits to [Andy Duboc](https://github.com/andydbc) who provided the amazing [Hologram shaders](https://github.com/andydbc/HologramShader) used in this repo.