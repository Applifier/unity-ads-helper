using System;
using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

/// <summary>
/// Unity Ads Helper. Making integration in Unity a breeze!
/// </summary>
public class UnityAdsHelper : MonoBehaviour 
{
	/// <summary>
	/// Called when an ad is hidden. The ad was shown without being skipped. 
	/// Use this event for rewarding users.
	/// </summary>
	public static Action onFinished;
	/// <summary>
	/// Called when an ad is hidden. The ad was skipped while being shown. 
	/// Users should not be rewarded.
	/// </summary>
	public static Action onSkipped;
	/// <summary>
	/// Called when an error occurs while attempting to show an ad.
	/// </summary>
	public static Action onFailed;

	/// <summary>
	/// The gamerSID is a unique identifier used with Server-to-Server Redeem Callbacks.
	/// </summary>
	/// <value>The gamerSID.</value>
	public static string gamerSID { get { return _gamerSID; } set { _gamerSID = Validate(value); }}

	private static string _gamerSID;

	private static string Validate (string value)
	{
		if (value != null) value = value.Trim();
		if (string.IsNullOrEmpty(value)) value = null;

		return value;
	}

#if UNITY_IOS || UNITY_ANDROID

	private static bool _isInitializing;

	private static UnityAdsHelper _instance;
	private static UnityAdsHelper GetInstance ()
	{
		if (_instance == null)
		{
			GameObject gO = GameObject.Find("UnityAdsHelper");
			if (gO == null) gO = new GameObject("UnityAdsHelper");

			_instance = gO.GetComponent<UnityAdsHelper>();
			if (_instance == null) _instance = gO.AddComponent<UnityAdsHelper>();
		}
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this)
		{
			Debug.LogWarning("An instance of UnityAdsHelper already exists. Duplicate will be destroyed.");
			Destroy(this);
		}

		if (!isInitialized && !_isInitializing) Initialize();

		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Initializes the Unity Ads SDK with <see cref="UnityAdsSettings"/>. 
	/// To configure settings, go to Edit > Unity Ads Settings in the Unity Editor menu.
	/// </summary>
	public static void Initialize () 
	{ 
	#if !UNITY_ADS
		if (_isInitializing)
		{
			Debug.LogWarning("Unity Ads is already being initialized.");
			return;
		}
		else _isInitializing = true;

		if (isInitialized)
		{
			Debug.LogWarning("Unity Ads is already initialized.");
			_isInitializing = false;
			return;
		}
		else if (!isSupported)
		{
			Debug.LogWarning("Unity Ads is not supported on the current runtime platform.");
			_isInitializing = false;
			return;
		}
		else if (GetInstance() == null)
		{
			Debug.LogError("Failed to initialize the UnityAdsHelper. Instance not found.");
			_isInitializing = false;
			return;
		}
		else _instance.DoInitialize();

	#else
		if (isInitialized)
		{
			Debug.Log("Unity Ads is initialized.");
		}
		else Debug.LogWarning("Unity Ads is not enabled. See the Connect window in Unity for details.");
	#endif
	}
	
	private void DoInitialize ()
	{
		Debug.Log("Preparing for Unity Ads initialization...");

		UnityAdsSettings settings = (UnityAdsSettings)Resources.Load("UnityAdsSettings");

		if (settings == null)
		{
			Debug.LogError("Failed to initialize Unity Ads. Settings file not found.");
			_isInitializing = false;
			return;
		}

		string gameId = null;
		
	#if UNITY_IOS
		gameId = settings.iosGameId.Trim();
	#elif UNITY_ANDROID
		gameId = settings.androidGameId.Trim();
	#endif

		if (string.IsNullOrEmpty(gameId))
		{
			Debug.LogError("Failed to initialize Unity Ads. A valid game ID is required.");
			_isInitializing = false;
			return;
		}

		Advertisement.debugLevel = Advertisement.DebugLevel.None;

		if (settings.showInfoLogs)    Advertisement.debugLevel |= Advertisement.DebugLevel.Info;
		if (settings.showDebugLogs)   Advertisement.debugLevel |= Advertisement.DebugLevel.Debug;
		if (settings.showWarningLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.Warning;
		if (settings.showErrorLogs)   Advertisement.debugLevel |= Advertisement.DebugLevel.Error;
		
		if (settings.enableTestMode && !Debug.isDebugBuild)
		{
			Debug.LogWarning("Development Build must be enabled in Build Settings to enable Test Mode for Unity Ads.");
		}
		
		bool isTestModeEnabled = Debug.isDebugBuild && settings.enableTestMode;
		Debug.Log(string.Format("Initializing Unity Ads for game ID {0} with Test Mode {1}...",
			gameId, isTestModeEnabled ? "enabled" : "disabled"));
		
		Advertisement.Initialize(gameId,isTestModeEnabled);
		
		StartCoroutine(LogWhenUnityAdsIsInitialized());
	}
	
	private IEnumerator LogWhenUnityAdsIsInitialized ()
	{
		float initStartTime = Time.time;
		
		do yield return new WaitForSeconds(0.1f);
		while (!Advertisement.isInitialized);
		
		Debug.Log(string.Format("Unity Ads was initialized in {0:F1} seconds.",Time.time - initStartTime));
		_isInitializing = false;
		yield break;
	}

	/// <summary>
	/// Gets a value indicating whether an ad is currently showing.
	/// </summary>
	/// <value><c>true</c> if is showing; otherwise, <c>false</c>.</value>
	public static bool isShowing { get { return Advertisement.isShowing; }}
	/// <summary>
	/// Gets a value indicating whether Unity Ads is supported in the current Unity player. 
	/// Supported players include iOS, Android, and the Unity Editor.
	/// </summary>
	/// <value><c>true</c> if is supported; otherwise, <c>false</c>.</value>
	public static bool isSupported { get { return Advertisement.isSupported; }}
	/// <summary>
	/// Gets a value indicating whether Unity Ads is initialized.
	/// </summary>
	/// <value><c>true</c> if is initialized; otherwise, <c>false</c>.</value>
	public static bool isInitialized { get { return Advertisement.isInitialized; }}
	
	/// <summary>
	/// Determines if Unity Ads is initialized and ready to show an ad using the default ad placement zone.
	/// </summary>
	/// <returns><c>true</c> if is ready; otherwise, <c>false</c>.</returns>
	public static bool IsReady () { return IsReady(null); }
	/// <summary>
	/// Determines if Unity Ads is initialized and ready to show an ad for the specified ad placement zone ID.
	/// To use the default ad placement zone, pass in a <c>null</c> value for the zone ID.
	/// </summary>
	/// <returns><c>true</c> if is ready; otherwise, <c>false</c>.</returns>
	/// <param name="zoneId">Ad placment zone ID.</param>
	public static bool IsReady (string zoneId) 
	{
		return Advertisement.IsReady(Validate(zoneId));
	}

	/// <summary>
	/// Shows an ad using the default ad placement zone.
	/// </summary>
	public static void ShowAd () { ShowAd(null); }
	/// <summary>
	/// Shows an ad using the specified ad placement zone ID.
	/// To use the default ad placement zone, pass in a <c>null</c> value for the zone ID.
	/// </summary>
	/// <param name="zoneId">Ad placement zone ID.</param>
	public static void ShowAd (string zoneId)
	{
		zoneId = Validate(zoneId);

		if (Advertisement.IsReady(zoneId))
		{
			Debug.Log("Showing ad now...");
			
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			options.gamerSid = null; // When using S2S Redeem Callbacks, ignore callbacks without an 'sid' parameter value.

			Advertisement.Show(zoneId,options);
		}
		else 
		{
			Debug.LogWarning(string.Format("Unable to show ad. The ad placement zone {0} is not ready.",
				zoneId == null ? "default" : zoneId));
		}
	}

	/// <summary>
	/// Shows a rewarded ad using the default ad placement zone.
	/// </summary>
	public static void ShowRewardedAd () { ShowAd(null); }
	/// <summary>
	/// Shows a rewarded ad using the specified ad placement zone ID.
	/// To use the default ad placement zone, pass in a <c>null</c> value for the zone ID.
	/// </summary>
	/// <param name="zoneId">Ad placement zone ID.</param>
	public static void ShowRewardedAd (string zoneId)
	{
		zoneId = Validate(zoneId);

		if (Advertisement.IsReady(zoneId))
		{
			Debug.Log("Showing ad now...");

			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			options.gamerSid = _gamerSID;

			Advertisement.Show(zoneId,options);
		}
		else 
		{
			Debug.LogWarning(string.Format("Unable to show ad. The ad placement zone {0} is not ready.",
				zoneId == null ? "default" : zoneId));
		}
	}
	
	private static void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			if (onFinished != null) onFinished();
			break;
		case ShowResult.Skipped:
			Debug.LogWarning("The ad was skipped before reaching the end.");
			if (onSkipped != null) onSkipped();
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			if (onFailed != null) onFailed();
			break;
		}

		ClearActions();
	}

	private static void ClearActions ()
	{
		onFinished = null;
		onSkipped = null;
		onFailed = null;
	}
#else

	public static void Initialize () 
	{
		Debug.LogError("Failed to initialize Unity Ads. Current build platform is not supported.");
	}

	public static bool isShowing { get { return false; }}
	public static bool isSupported { get { return false; }}
	public static bool isInitialized { get { return false; }}
	
	public static bool IsReady () { return false; }
	public static bool IsReady (string zoneId) { return false; }
	
	public static void ShowAd () { ShowAd(null); }
	public static void ShowAd (string zoneId) 
	{
		Debug.LogError("Failed to show ad. Unity Ads does not support the current build platform.");
	}

#endif
}
