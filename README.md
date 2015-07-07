# Unity Ads Helper

The Unity Ads Helper is designed to streamline the integration of Unity Ads in Unity projects.

## Outline

* [Features](#features)
* [Getting Started](#getting-started)
* [Using UnityAdsHelper](#using-unityadshelper)
  * [Initializing Unity Ads](#initializing-unity-ads)
  * [Creating a Unity UI Button](#creating-a-unity-ui-button)
  * [Using a Unity UI Button to Show Ads](#using-a-unity-ui-button-to-show-ads)
  * [Rewarding Users for Watching Ads](#rewarding-users-for-watching-ads)
* [Example Assets](#example-assets)
* [Scripting API](#scripting-api)

## Features

* Use of XML comments for documenting public fields and methods.  
* Settings are stored as a ScriptableObject available from the Unity Editor menu.  
* A robust `Initialize` method that applies settings and logs when done initializing.  
* A robust `ShowAd` method that applies options and handles the result callback.  
* Actions for handling show results:  
  * `OnFinished`  
  * `OnSkipped`  
  * `OnFailed`  
* A `SetGamerSID` method for integrations using Server-to-Server Redeem Callbacks.  
* Improved handling of common integration issues:  
  * Unity Ads is only initialized once per game session.  
  * Test Mode is tied to the Development Build option in Build Settings to prevent from accidentally shipping a game with test mode still enabled.  
  * Game IDs are trimmed of white space and checked for `null` or empty values.  
  * Zone IDs are trimmed of white space and set to `null` if empty.  
  * JavaScript friendly methods.  
* Example assets included for reuse in your own project.  

[⇧ Back to top](#unity-ads-helper)

## Getting Started

**Step 1:** Download and import the [UnityAdsHelper.unitypackage](UnityAdsHelper.unitypackage?raw=true) into your Unity project.

> _**Note:** Unity Ads assets are not included. Please download and import the latest version of the [Unity Ads asset package](https://www.assetstore.unity3d.com/en/#!/content/21027) from the Asset Store._

**Step 2:** Set the **Platform** to either **iOS** or **Android**:

1. Select **File > Build Settings...** from the Unity Editor menu.  
1. Select **iOS** or **Android** from the **Platform** list.
1. Select **Switch Platform**.

![Build Settings](images/build-settings.png)

**Step 3:** Enable **Development Build** to allow for the use of **Test Mode** with Unity Ads:

1. Select **File > Build Settings...** from the Unity Editor menu.  
1. Check the box next to **Development Build** to enable.

**Step 4:** Configure Unity Ads for your project:

1. Select **Edit > Unity Ads Settings** from the Unity Editor menu.  
1. Enter your **iOS Game ID** and **Android Game ID** in the fields provided.  

![Unity Ads Settings](images/unity-ads-settings.png)

#### Developing with JavaScript?
Move the **UnityAdsHelper** directory into the **Standard Assets** directory.

[⇧ Back to top](#unity-ads-helper)

## Using UnityAdsHelper

A simple Unity Ads integration can be summed up in just 3 steps:

1. Initialize the Unity Ads SDK.  
1. Determine if an ad is ready.  
1. Show the ad.  

In this example, we'll show you how to initialize Unity Ads using the [UnityAdsHelper](Assets/UnityAdsHelper/Scripts/UnityAdsHelper.cs) script. Then we'll guide you through setting up a Unity UI Button that will show an ad using a method that utilizes the UnityAdsHelper script.

[⇧ Back to top](#unity-ads-helper)

### Initializing Unity Ads

![Unity Ads Settings](images/menu-item-banner.png)

The `UnityAdsHelper.Initialize()` method configures and initializes Unity Ads by referencing the settings stored in [UnityAdsSettings](Assets/UnityAdsHelper/Scripts/UnityAdsSettings.cs), a ScriptableObject. By default, the UnityAdsSettings asset can be found in the Resources directory. You can view the UnityAdsSettings asset in the Inspector by selecting it directly, or by selecting **Edit > Unity Ads Settings** from the Unity Editor menu. Selecting this menu item will create the UnityAdsSettings asset if it does not already exist in your project.

The option **Enable Test Mode** is enabled by default in UnityAdsSettings. While developing and testing your game, you should always leave Test Mode enabled. The only time it's appropriate to disable Test Mode is in cases where you are attempting to test the functionality of production ad campaigns, instead of just test ad campaigns.

> _**Note:** Development Build must be enabled in Build Settings in order to initialize Unity Ads with Test Mode enabled. This is a feature of the UnityAdsHelper script, and is intended to help prevent from accidentally releasing a final build with Test Mode still enabled._

![Example Screenshots](images/example-screenshots.png)

One of the nice things about the UnityAdsHelper script is that it can be used to initialize Unity Ads in one of two ways:

* by calling `UnityAdsHelper.Initialize()` from a script within your project,
* or simply by adding the UnityAdsHelper script to a new GameObject in your main scene.

For the purposes of this example, let's initialize Unity Ads by calling `UnityAdsHelper.Initialize()`.

**C# Example – UnityAdsExample.cs**  
```csharp
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

**JavaScript Example – UnityAdsExample.js**  
```javascript
#pragma strict

public class UnityAdsExample extends MonoBehaviour
{
	function Start () : void
	{
		UnityAdsHelper.Initialize();
	}
}
```

Now let's add the script to your scene. Create a new GameObject in your main scene and rename it to _UnityAdsExample_. Then add the UnityAdsExample script to it.

The UnityAdsHelper should only be initialized once within your game, ideally when your game first starts up. In any case, the UnityAdsHelper is capable of managing its own instances, and persists across scene loads. So there will only ever be one UnityAdsHelper at a time in your game, and Unity Ads will only be initialized once.

> ##### Pro Tip: Designing for User Experience
>
> One thing to consider when designing your game with Unity Ads is that you don't always have to initialize Unity Ads at the start of your game.
>
> For instance, if you've designed your game to delay the showing of ads until after the user has had time to learn the rules of gameplay and progress through a few levels, it may take a few game sessions before they reach a point where they would start seeing ads.
>
> In this case, you could hold off on initializing Unity Ads until they've crossed this threshold. Keep in mind though, Unity Ads does take several seconds to initialize and cache the assets necessary to show an ad. You should therefore allow sufficient time for Unity Ads to finish initializing before showing an ad.

[⇧ Back to top](#unity-ads-helper)

### Creating a Unity UI Button

![Unity UI Button](images/button-ui-banner.png)

In this section, we'll create and configure a Unity UI Button for use with this example.

**Step 1:** Start by creating a new Unity UI Button in your scene. Creating a new UI Button will also add a UI Canvas and EventSystem to your scene if they don't already exist.

1. Select **GameObject > UI > Button** from the Unity Editor menu.
1. Press the **T key** to switch to using the Rect Transform Tool.
1. Select and drag the UI Button to the center of the UI Canvas.

**Step 2:** Customize the UI Button for use with this example.

1. Rename the new UI Button to _ShowAdButton_.
1. Expand _ShowAdButton_ in the Hierarchy to view child objects.
1. Locate and select the GameObject named _Text_.
1. Rename the _Text_ GameObject to _ReadyText_.
1. Enter "Show Default Ad" into the **Text** field of the Text component.

![Example Scene](images/example-scene.png)

**Step 3:** Create a non-interactable version of the button text.

1. Locate and select the GameObject named _ReadyText_.
1. Select **Edit > Duplicate** to create a duplicate GameObject.
1. Rename the duplicate GameObject to _WaitingText_.
1. Enter "Waiting..." into the **Text** field of the the Text component.
1. Disable the Text component of the GameObject for the time being.

![Example Hierachy](images/example-hierarchy.png)
![Example ReadyText](images/example-waitingtext.png)

**Step 4:** Configure the UI Canvas to scale with screen size.

1. Locate and select the GameObject named _Canvas_.
1. Set the **UI Scale Mode** of the Canvas Scaler to **Scale With Screen Size**.
1. Set the **Match** value to **0.5** between Width and Height.

![Example Canvas Scaler](images/example-canvas-scaler.png)

> _**Note:** Be sure to check out the Unity UI system [Tutorials](http://unity3d.com/learn/tutorials/modules/beginner/ui) and [Docs](http://docs.unity3d.com/Manual/UISystem.html) to learn more._

[⇧ Back to top](#unity-ads-helper)

### Using a Unity UI Button to Show Ads

![Button Example](images/button-example-banner.png)

With the Unity UI system all setup, let's write a script that can be used to show an ad with the UI Button's OnClick UnityEvent, and make the UI Button interactable only when ads are ready.

**Step 1:** Create a new script called _ButtonExample_ and define a `ShowAd` method.

Defining the `zoneId` as a public variable allows us to easily update it from the Inspector. If a zone ID is not specified, the default zone will be used instead.

> _**Note:** A full list of available zone IDs can be found in the [Unity Ads Admin](http://unityads.unity3d.com/admin) under the Monetization Settings tab of your game profile._

**C# Example – ButtonExample.cs**  
```csharp
using UnityEngine;
using System.Collections;

public class ButtonExample : MonoBehaviour
{
	public string zoneId;

	public void ShowAd ()
	{
		UnityAdsHelper.ShowAd(zoneId);
	}
}
```

**JavaScript Example – ButtonExample.js**  
```javascript
#pragma strict

public class ButtonExample extends MonoBehaviour
{
	public var zoneId : String;

	public function ShowAd () : void
	{
		UnityAdsHelper.ShowAd(zoneId);
	}
}
```

**Step 2:** Add the ButtonExample script to the UI Button in your scene.

1. Locate and select the _ShowAdButton_ GameObject.
1. Select **Component > Scripts > Button Example** from the Unity Editor menu.

**Step 3:** Configure the UI Button's OnClick UnityEvent.

1. Locate and select the _ShowAdButton_ GameObject.
1. Locate the OnClick section of the Button componenet.
1. Press the **+ button** under the OnClick section to add a reference.
1. Select and drag the _ShowAdButton_ GameObject from the Hierarchy to the target GameObject field.
1. Select **ButtonExample > ShowAd ()** method from the dropdown menu.

![Example OnClick UnityEvent](images/example-onclick.png)

> _**Note:** At this point you can run the game to test out the button. But keep in mind, we're not yet checking to see if ads are ready. We'll do this in the following steps._

**Step 4:** Update the ButtonExample script to handle the interactable state of the UI Button.

**C# Example – ButtonExample.cs**  
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonExample : MonoBehaviour
{
	public Text textReady;
	public Text textWaiting;

	public string zoneId;

	private Button _button;

	void Start ()
	{
		_button = GetComponent<Button>();
	}

	void Update ()
	{
		if (_button)
		{
			_button.interactable = UnityAdsHelper.IsReady(zoneId);

			if (textReady) textReady.enabled = _button.interactable;
			if (textWaiting) textWaiting.enabled = !_button.interactable;
		}
	}

	public void ShowAd ()
	{
		UnityAdsHelper.ShowAd(zoneId);
	}
}
```

**JavaScript Example – ButtonExample.js**  
```javascript
#pragma strict
import UnityEngine.UI;

public class ButtonExample extends MonoBehaviour
{
	public var textReady : Text;
	public var textWaiting : Text;

	public var zoneId : String;

	private var _button : Button;

	function Start () : void
	{
		_button = GetComponent.<Button>();
	}

	function Update () : void
	{
		if (_button)
		{
			_button.interactable = UnityAdsHelper.IsReady(zoneId);

			if (textReady) textReady.enabled = _button.interactable;
			if (textWaiting) textWaiting.enabled = !_button.interactable;
		}
	}

	public function ShowAd () : void
	{
		UnityAdsHelper.ShowAd(zoneId);
	}
}
```

**Step 5:** Add UI Text components to the **Text Ready** and **Text Waiting** fields.

1. Locate and select the _ShowAdButton_ GameObject.
1. Expand _ShowAdButton_ in the Hierarchy to view child objects.
1. Select and drag the _ReadyText_ GameObject to the **Text Ready** field.
1. Select and drag the _WaitingText_ GameObject to the **Text Waiting** field.

![Example Button Script](images/example-button-script.png)

Now let's press the **Play** button in the Unity Editor toolbar to run the scene.

Once Unity Ads is initialized, and ad is ready to be shown, the _ShowAdButton_ will become interactable. Pressing the _ShowAdButton_ to show an ad will show a blue placeholder image while in the Unity Editor.

Video ads are only shown when running on iOS or Android devices.

> _**Note:** When running on device, the Unity player is paused while an ad is shown, then un-paused when the ad is hidden. However, in the Unity Editor, the Unity player is not paused._
>
> _To get the same effect in the Unity Editor, you would need to pause the AudioListener and set `Time.timeScale = 0` while the placeholder is shown. Then restore the `Time.timeScale` value and un-pause the AudioListener when the placeholder is hidden._
>
> _You can use `UnityAdsHelper.isShowing` to determine when to pause and un-pause your game while in the Unity Editor._
>
>     #if UNITY_EDITOR
>     AudioListener.pause = UnityAdsHelper.isShowing;
>     Time.timeScale = UnityAdsHelper.isShowing ? 0f : 1f;
>     #endif

[⇧ Back to top](#unity-ads-helper)

### Rewarding Users for Watching Ads

![Example Rewarded Ad](images/example-rewarded-banner.png)

Since rewarded ads are typically non-skippable, some form of button or prompt should always be used to show a rewarded ad. Doing so presents your users with the choice to opt-in, which can lead to a better user experience while making ad impressions more effective.

However, when offering rewarded ads, you may also want to limit how often users are able to redeem rewards for watching ads. In this case, you could implement a cooldown between ads.

Let's update the the ButtonExample script with a method to reward users and set the cooldown for the next ad. We will assign this method to the `OnFinished`, which is only called when an ad is watched but not skipped.

**C# Example – ButtonExample.cs**  
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonExample : MonoBehaviour
{
	public Text textReady;
	public Text textWaiting;

	public string zoneId;

	public float cooldownTime = 300f;
	public int rewardAmount = 250;

	private float _cooldownTime;

	private Button _button;

	void Start ()
	{
		_button = GetComponent<Button>();
	}

	void Update ()
	{
		if (_button)
		{
			_button.interactable = IsReady();

			if (textReady) textReady.enabled = _button.interactable;
			if (textWaiting) textWaiting.enabled = !_button.interactable;
		}
	}

	private bool IsReady ()
	{
		if (Time.time > _cooldownTime)
		{
			return UnityAdsHelper.IsReady(zoneId);
		}
		else return false;
	}

	public void ShowAd ()
	{
		UnityAdsHelper.OnFinished = OnFinished;
		UnityAdsHelper.ShowAd(zoneId);
	}

	private void OnFinished ()
	{
		if (rewardAmount > 0)
		{
			Debug.Log("The player has earned a reward!");
		}

		if (cooldownTime > 0)
		{
			_cooldownTime = Time.time + cooldownTime;
			Debug.Log(string.Format("Next ad is available in {0} seconds.",cooldownTime));
		}
	}
}
```

**JavaScript Example – ButtonExample.js**  
```javascript
#pragma strict
import UnityEngine.UI;

public class ButtonExample extends MonoBehaviour
{
	public var textReady : Text;
	public var textWaiting : Text;

	public var zoneId : String;

	public var cooldownTime : float = 300f;
	public var rewardAmount : int = 250;

	private var _cooldownTime : float;

	private var _button : Button;

	function Start () : void
	{
		_button = GetComponent.<Button>();
	}

	function Update () : void
	{
		if (_button)
		{
			_button.interactable = IsReady();

			if (textReady) textReady.enabled = _button.interactable;
			if (textWaiting) textWaiting.enabled = !_button.interactable;
		}
	}

	private function IsReady () : boolean
	{
		if (Time.time > _cooldownTime)
		{
			return UnityAdsHelper.IsReady(zoneId);
		}
		else return false;
	}

	public function ShowAd () : void
	{
		UnityAdsHelper.OnFinished = OnFinished;
		UnityAdsHelper.ShowAd(zoneId);
	}

	private function OnFinished () : void
	{
		if (rewardAmount > 0)
		{
			Debug.Log("The player has earned a reward!");
		}

		if (cooldownTime > 0)
		{
			_cooldownTime = Time.time + cooldownTime;
			Debug.Log(String.Format("Next ad is available in {0} seconds.",cooldownTime));
		}
	}
}
```

With the ButtonExample script updated, you can now set the **Cooldown Time** (in seconds) and **Reward Amount** from the Inspector. Be sure to also specify the zone ID of your rewarded zone in the **Zone ID** field.

![Example Rewarded](images/example-rewarded.png)

> _**Note:** To continue using this script with non-rewarded ads, simply set **Cooldown Time** and **Reward Amount** to 0. Leave the **Zone ID** field empty to use the default zone._

![Example Default](images/example-default.png)

> ##### Pro Tip: Improving eCPM through Rewarded Ads
>
> The eCPM is defined as the Effective Cost Per 1000 impressions.
>
>     eCPM = revenue / starts * 1000
>
> The eCPM is essentially a performance value used to indicate the effectiveness of ads shown in your game. The more impressions you have, the more reliable this value is.
>
> Statistically speaking, the first few ads shown tend to be the most effective in generating revenue. While the most successful ad implementations knowingly take advantage of this by only showing a few ads per user per day, they also make those impressions count.
>
> Video ads are most effective when players are willing to watch. An obvious statement, but it's safe to say the average user doesn't play your game just so they can watch ads. So where's the incentive?
>
> Using rewarded ads is a way to provide users with the motivation to watch non-skippable ads, in exchange for an in-game reward. For example, some games use rewarded ads to give an extra life or a second chance, allowing the user to continue gameplay from a fail state without having to restart from the very beginning. Another example might be to offer the user some amount of in-game currency or consumable item that could be applied toward upgrading their character or purchasing new equipment.
>
> Ultimately, using rewarded ads is a great way to effectively show ads, while also making them relevant to your game and less disruptive to the flow of the in-game experience.

[⇧ Back to top](#unity-ads-helper)

## Example Assets

![Example Assets](images/example-assets-banner.png)

Example assets are located in [Assets/UnityAdsHelper/Examples](Assets/UnityAdsHelper/Examples/):

* UnityAdsExample.unity - Example scene file.  
* [UnityAdsExample.cs](Assets/UnityAdsHelper/Examples/Scripts/UnityAdsExample.cs) - The main example script used to initialize UnityAdsHelper.  
* [ButtonExample.cs](Assets/UnityAdsHelper/Examples/Scripts/ButtonExample.cs) - A Unity UI Button example script used to show rewarded ads.
  * Specify a zone ID or leave blank to use the default zone.  
  * Set a cooldown time in seconds to control how often users can watch rewarded ads.
  * Set a reward amount the user should receive after watching an ad without skipping.
  * Cooldowns persist across scenes and game sessions.
  * Cooldowns are independent of each other.
* [ShowAdOnLoad.cs](Assets/UnityAdsHelper/Examples/Scripts/ShowAdOnLoad.cs) - Script for showing an ad when the scene loads.
  * Specify a zone ID or leave blank to use the default zone.
  * Set a timeout duration in seconds to allow for initialization.
  * Set a timeout duration in seconds to allow ads to become ready.
  * Set a yield time in seconds for how often `isInitialized` and `IsReady()` will be evaluated.
  * The process of showing and ad on load will be canceled if either timeout is exceeded.

These assets are reusable. Feel free to modify them for use in your own project.

[⇧ Back to top](#unity-ads-helper)

## Scripting API

### UnityAdsHelper

class / Inherits from: [MonoBehaviour](http://docs.unity3d.com/ScriptReference/MonoBehaviour.html)

#### Static Properties

* ##### isSupported
  `public static bool isSupported { get; }`  

  Gets a value indicating whether Unity Ads is supported in the current Unity player.

* ##### isInitialized
  `public static bool isInitialized { get; }`  

  Gets a value indicating whether Unity Ads is initialized.

* ##### isShowing
  `public static bool isShowing { get; }`  

  Gets a value indicating whether an ad is currently showing.

* ##### gamerSID
  `public static string gamerSID { get; }`

  Gets the gamerSID, a unique identifier used with Server-to-Server Redeem Callbacks.

#### Static Methods

* ##### Initialize
  `public static void Initialize ();`  

  Configures and initializes Unity Ads using an instance of `UnityAdsSettings`. To configure Unity Ads settings, select **Edit > Unity Ads Settings** from the Unity Editor menu.

* ##### IsReady
  `public static bool IsReady ();`  
  `public static bool IsReady (string zoneId);`  

  Determines if Unity Ads is initialized and ready to show an ad using the specified `zoneId`. If the `zoneId` is not specified, or is set to `null`, the default ad placement zone will be used instead.

* ##### ShowAd
  `public static void ShowAd ();`  
  `public static void ShowAd (string zoneId);`  

  Shows an ad using the specified `zoneId`. If the `zoneId` is not specified, or is set to `null`, the default ad placement zone will be used instead.

* ##### SetGamerSID  
  `public static void SetGamerSID (string gamerSID);`

  Sets the gamer SID parameter, a unique identifier used with Server-to-Server Redeem Callbacks.

#### Static Actions

* ##### OnFinished
  `public static Action OnFinished;`  

  Called when an ad is hidden. The ad was shown without being skipped.

* ##### OnSkipped
  `public static Action OnSkipped;`  

  Called when an ad is hidden. The ad was skipped while being shown.

* ##### OnFailed
  `public static Action OnFailed;`  

  Called when an error occurs while attempting to show an ad.

[⇧ Back to top](#unity-ads-helper)
