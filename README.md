# Unity Ads Helper
The Unity Ads Helper is designed to streamline and manage the integration of Unity Ads in Unity projects.

Features include:  
* Unity Ads settings are stored as a `ScriptableObject` available from the Unity Editor menu.  
* A robust `Initialize` method that applies settings and logs when Unity Ads is initialized.  
* A robust `ShowAd` method that applies options and handles and result callbacks.  
* Exposure to callbacks for handling show results:  
  * `onFinishedEvent`  
  * `onSkippedEvent`  
  * `onFailedEvent`  
* A `SetGamerSID` method for integrations using Server-to-Server Redeem Callbacks.  
* Error handling for common integration issues:  
  * Game IDs are trimmed of spaces and checked for `null` or empty values.  
  * Zone IDs are trimmed of spaces and set to `null` if emtpy.  
  * JavaScript friendly methods.  

## Getting Started

#### Step 1
Download and import the [UnityAdsHelper.unitypackage](UnityAdsHelper.unitypackage?raw=true) into your Unity project.

_**Note:** This project does not include Unity Ads. Download and import [the latest version from the Asset Store](https://www.assetstore.unity3d.com/en/#!/content/21027)._

#### Step 2
Set the **Platform** to either **iOS** or **Android**:  
1. Select **File > Build Settings...** from the Unity Editor menu.  
2. Select **iOS** or **Android** from the **Platform** list.
3. Select **Switch Platform**.

#### Step 3
Enable **Development Build** to allow the use of **Test Mode** with Unity Ads:  
1. Select **File > Build Settings...** from the Unity Editor menu.  
2. Mark the check box next to **Development Build** to enable.

#### Step 4
Configure Unity Ads for your project:  
1. Select **Edit > Unity Ads Settings** from the Unity Editor menu.  
2. Enter your **iOS Game ID** and **Android Game ID** in the fields provided.  

#### Developing with JavaScript? 
Move the **UnityAdsHelper** directory into **Standard Assets**.

## Using UnityAdsHelper

Example code in C#:  
```CSharp
// Placeholder for exampe code in C#.
```

Example code in JavaScript:  
```JavaScript
// Placeholder for Example code in JavaScript.
```

## Scripting API

### UnityAdsHelper
class / Inherits from: [MonoBehaviour](http://docs.unity3d.com/ScriptReference/MonoBehaviour.html)

#### Static Properties

##### UnityAdsHelper.isShowing
`public static bool isShowing { get; }`  

Gets a value indicating whether an ad is currently showing.

##### UnityAdsHelper.isSupported
`public static bool isSupported { get; }`  

Gets a value indicating whether Unity Ads is supported in the current Unity player.

##### UnityAdsHelper.isInitialized
`public static bool isInitialized { get; }`  

Gets a value indicating whether Unity Ads is initialized.

#### Static Functions

##### UnityAdsHelper.Initialize
`public static void Initialize ();`  

Configures and initializes Unity Ads using an instance of `UnityAdsSettings`. To configure Unity Ads settings, select **Edit > Unity Ads Settings** from the Unity Editor menu.

##### UnityAdsHelper.IsReady
`public static bool IsReady ();`  
`public static bool IsReady (string zoneId);`  

Determines if Unity Ads is initialized and ready to show an ad using the specified `zoneId`. If the `zoneId` is not specified, or is set to `null`, the default ad placement zone will be used instead.

##### UnityAdsHelper.ShowAd
`public static void ShowAd ();`  
`public static void ShowAd (string zoneId);`  

Shows an ad using the specified `zoneId`. If the `zoneId` is not specified, or is set to `null`, the default ad placement zone will be used instead.

#### Static Events

##### UnityAdsHelper.onFinishedEvent
`public static Action onFinishedEvent;`  

Called when an ad is hidden. The ad was shown without being skipped.

##### UnityAdsHelper.onSkippedEvent
`public static Action onSkippedEvent;`  

Called when an ad is hidden. The ad was skipped while being shown.

##### UnityAdsHelper.onFailedEvent
`public static Action onFailedEvent;`  

Called when an error occurs while attempting to show an ad.
