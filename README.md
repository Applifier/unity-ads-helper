# Unity Ads Helper
The Unity Ads Helper is designed to streamline the integration of Unity Ads in Unity projects.

Features include:  
* Unity Ads settings are stored as a `ScriptableObject` available from the Unity Editor menu.  
* A robust `Initialize` method that applies settings and logs when Unity Ads is initialized.  
* A robust `ShowAd` method that applies options and handles and result callbacks.  
* Events for handling show results:  
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
Move the **UnityAdsHelper** directory into the **Standard Assets** directory.

## Using UnityAdsHelper

A simple Unity Ads integration can be summed up in just 3 steps:

1. Initialize the Unity Ads SDK.  
2. Determine if an ad is ready.  
3. Show the ad.  

In this example, we'll show you how to initialize Unity Ads using the UnityAdsHelper script. Then we'll guide you through setting up a Unity UI Button that will show an ad using a method that utilizes the UnityAdsHelper script.

### Initializing the UnityAdsHelper

One of the nice things about the UnityAdsHelper script is that it can be initialized by calling the `Initialize()` method from one of your game scripts, or simply by adding it as a component to a `GameObject` in your scene.

For the purposes of this example, let's initialize the UnityAdsHelper with our UnityAdsExample script.

**UnityAdsExample.cs**  
```CSharp
using UnityEngine;
using System.Collections;

public class UnityAdsExample : MonoBehaviour 
{
	void Start ()
	{
		UnityAdsHelper.Initialize();
	}
}
```

**UnityAdsExample.js**  
```JavaScript
#pragma strict

public class UnityAdsExample extends MonoBehaviour
{
	function Start : void ()
	{
		UnityAdsHelper.Initialize();
	}
}
```

Ideally, you should only initialize the UnityAdsHelper once when your game first starts up. In any case, the UnityAdsHelper manages its own instances. There will only ever be one UnityAdsHelper at a time in your game. 

> **Pro Tip: Designing for User Experience**
> 
> One thing to be aware of is that you don't always have to initialize Unity Ads at the start of your game. 
> 
> For instance, if you've designed your game to delay the showing of ads until after the user has had time to learn the rules of gameplay and progress through a few levels, it may take a few game sessions before they reach a point where they would start seeing ads. 
> 
> In this case, you can hold off on initializing Unity Ads until they've crossed this threshold. Keep in mind though, Unity Ads does take several seconds to initialize and cache the assets neccessary to show an ad. You should allow sufficient time for Unity Ads to finish initializing before showing an ad.

### Creating a Unity UI Button

### Using a Unity UI Button to Show Ads

## Unity Ads Demo

Included with this project is a [UnityAdsDemo.scene](Assets/UnityAdsHelper/Demo/UnityAdsDemo.scene) and the following demo scripts:  
* [UnityAdsDemo.cs](Assets/UnityAdsHelper/Demo/Scripts/UnityAdsDemo.cs)  
* [UnityAdsButton.cs](Assets/UnityAdsHelper/Demo/Scripts/UnityAdsButton.cs)  
* [ShowAdOnLoad.cs](Assets/UnityAdsHelper/Demo/Scripts/ShowAdOnLoad.cs)

Look them over. Try them out. And feel free to modify them for use in your own project.

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
