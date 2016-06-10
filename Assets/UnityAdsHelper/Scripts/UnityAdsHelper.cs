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

	public static UnityAdsHelper Instance
	{
		get 
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
	}

	public static UnityAdsSettings Settings
	{
		get 
		{
			if (_settings == null) _settings = (UnityAdsSettings)Resources.Load("UnityAdsSettings");

			return _settings;
		}
	}

	private static UnityAdsHelper _instance;
	private static UnityAdsSettings _settings;
	private static bool _isInitializing;
	private static string _gamerSID;

	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this)
		{
			Debug.LogWarning("An instance of UnityAdsHelper already exists. Duplicate will be destroyed.");
			Destroy(this);
		}

		DontDestroyOnLoad(gameObject);
	}

	void Start ()
	{
		bool allowInit = false;

		#if UNITY_IOS || UNITY_ANDROID
		if (Settings != null)
		{
			Advertisement.DebugLevel debugLevel = Advertisement.DebugLevel.None;

			if (Settings.showInfoLogs)    debugLevel |= Advertisement.DebugLevel.Info;
			if (Settings.showDebugLogs)   debugLevel |= Advertisement.DebugLevel.Debug;
			if (Settings.showWarningLogs) debugLevel |= Advertisement.DebugLevel.Warning;
			if (Settings.showErrorLogs)   debugLevel |= Advertisement.DebugLevel.Error;

			Advertisement.debugLevel = debugLevel;

			allowInit = _settings.overrideAdsServiceInit;
		}
		#endif

		#if !UNITY_ADS
		allowInit = true;
		#endif

		if (allowInit && !isInitialized && !_isInitializing) Initialize();
	}

	void OnDestroy ()
	{
		StopAllCoroutines();
	}

	/// <summary>
	/// Initializes the Unity Ads SDK with <see cref="UnityAdsSettings"/>. 
	/// To configure settings, go to Edit > Unity Ads Settings in the Unity Editor menu.
	/// </summary>
	public static void Initialize () 
	{
		#if UNITY_IOS || UNITY_ANDROID
		string gameId = null;

		if (Settings == null)
		{
			Debug.LogError("Failed to initialize Unity Ads. Settings file not found.");
			return;
		}

		#if UNITY_IOS
		gameId = Settings.iosGameId.Trim();
		#elif UNITY_ANDROID
		gameId = Settings.androidGameId.Trim();
		#endif

		Initialize(gameId,Settings.enableTestMode);

		#else
		Debug.LogError("Failed to initialize Unity Ads. Current build platform is not supported.");
		#endif
	}
	/// <summary>
	/// Initializes the Unity Ads SDK with <see cref="UnityAdsSettings"/>. 
	/// To configure settings, go to Edit > Unity Ads Settings in the Unity Editor menu.
	/// </summary>
	/// <param name="gameId">Platform specific game ID.</param>
	public static void Initialize (string gameId)
	{
		#if UNITY_IOS || UNITY_ANDROID
		bool enableTestMode = false;

		if (Settings != null)
		{
			enableTestMode = Settings.enableTestMode;
		}

		Initialize(gameId,enableTestMode);

		#else
		Debug.LogError("Failed to initialize Unity Ads. Current build platform is not supported.");
		#endif
	}
	/// <summary>
	/// Initializes the Unity Ads SDK with <see cref="UnityAdsSettings"/>. 
	/// To configure settings, go to Edit > Unity Ads Settings in the Unity Editor menu.
	/// </summary>
	/// <param name="gameId">Platform specific game ID.</param>
	/// <param name="enableTestMode">Test mode is enabled when <c>true</c>.</param>
	public static void Initialize (string gameId, bool enableTestMode) 
	{ 
		#if UNITY_IOS || UNITY_ANDROID
		if (isInitialized)
		{
			Debug.Log("Unity Ads is initialized.");
			return;
		}
		else if (_isInitializing)
		{
			Debug.LogWarning("Unity Ads is already being initialized.");
			return;
		}
		else if (!isSupported)
		{
			Debug.LogWarning("Unity Ads is not supported on the current runtime platform.");
			return;
		}
		else if (Settings == null)
		{
			Debug.LogError("Failed to initialize the UnityAdsHelper. Instance not found.");
			return;
		}
		else if (string.IsNullOrEmpty(gameId))
		{
			Debug.LogError("Failed to initialize Unity Ads. A valid game ID is required.");
			return;
		}
		else _isInitializing = true;
			
		Debug.Log("Preparing for Unity Ads initialization...");

		if (enableTestMode && !Debug.isDebugBuild)
		{
			Debug.LogWarning("Development Build must be enabled in Build Settings to enable Test Mode for Unity Ads.");
		}
		
		bool isTestModeEnabled = Debug.isDebugBuild && enableTestMode;
		Debug.Log(string.Format("Initializing Unity Ads for game ID {0} with Test Mode {1}...",
			gameId, isTestModeEnabled ? "enabled" : "disabled"));
		
		Advertisement.Initialize(gameId,isTestModeEnabled);
		
		Instance.StartCoroutine(Instance.LogWhenUnityAdsIsInitialized());

		#else
		Debug.LogError("Failed to initialize Unity Ads. Current build platform is not supported.");
		#endif
	}

	#if UNITY_IOS || UNITY_ANDROID
	private IEnumerator LogWhenUnityAdsIsInitialized ()
	{
		float initStartTime = Time.time;
		
		yield return new WaitWhile(() => !Advertisement.isInitialized);

		Debug.Log(string.Format("Unity Ads was initialized in {0:F1} seconds.",Time.time - initStartTime));
		_isInitializing = false;
		yield break;
	}
	#endif

	/// <summary>
	/// The gamerSID is a unique identifier used with Server-to-Server Redeem Callbacks.
	/// </summary>
	/// <value>The gamerSID.</value>
	public static string gamerSID 
	{ 
		get { return _gamerSID; } 
		set { _gamerSID = Validate(value); }
	}
	/// <summary>
	/// Gets a value indicating whether Unity Ads is supported in the current Unity player. 
	/// Supported players include iOS, Android, and the Unity Editor.
	/// </summary>
	/// <value><c>true</c> if is supported; otherwise, <c>false</c>.</value>
	public static bool isSupported 
	{ 
		get {
			#if UNITY_IOS || UNITY_ANDROID
			return Advertisement.isSupported;
			#else
			return false;
			#endif
		}
	}
	/// <summary>
	/// Gets a value indicating whether Unity Ads is being initialized.
	/// </summary>
	/// <value><c>true</c> if is being initialized; otherwise, <c>false</c>.</value>
	public static bool isInitializing 
	{ 
		get { return _isInitializing; }
	}
	/// <summary>
	/// Gets a value indicating whether Unity Ads is initialized.
	/// </summary>
	/// <value><c>true</c> if is initialized; otherwise, <c>false</c>.</value>
	public static bool isInitialized 
	{ 
		get {
			#if UNITY_IOS || UNITY_ANDROID
			return Advertisement.isInitialized;
			#else
			return false;
			#endif
		}
	}
	/// <summary>
	/// Gets a value indicating whether an ad is currently showing.
	/// </summary>
	/// <value><c>true</c> if is showing; otherwise, <c>false</c>.</value>
	public static bool isShowing 
	{ 
		get { 
			#if UNITY_IOS || UNITY_ANDROID
			return Advertisement.isShowing;
			#else
			return false;
			#endif
		}
	}

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
		#if UNITY_IOS || UNITY_ANDROID
		return Advertisement.IsReady(Validate(zoneId));
		#else
		return false;
		#endif
	}

	/// <summary>
	/// Shows an ad using the default ad placement zone.
	/// </summary>
	public static void ShowAd () { ShowAd(null,false); }
	/// <summary>
	/// Shows an ad using the specified ad placement zone ID.
	/// To use the default ad placement zone, pass in a <c>null</c> value for the zone ID.
	/// </summary>
	/// <param name="zoneId">Ad placement zone ID.</param>
	public static void ShowAd (string zoneId) { ShowAd(zoneId,false); }
	/// <summary>
	/// Shows an ad using the specified ad placement zone ID.
	/// To use the default ad placement zone, pass in a <c>null</c> value for the zone ID.
	/// </summary>
	/// <param name="zoneId">Ad placement zone ID.</param>
	/// <param name="rewarded">Sets the gamerSid option when <c>true</c>.</param> 
	public static void ShowAd (string zoneId, bool rewarded)
	{
		#if UNITY_IOS || UNITY_ANDROID
		zoneId = Validate(zoneId);

		if (Advertisement.IsReady(zoneId))
		{
			Debug.Log("Showing ad now...");

			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			options.gamerSid = rewarded ? _gamerSID : null;

			Advertisement.Show(zoneId,options);
		}
		else 
		{
			Debug.LogWarning(string.Format("Unable to show ad. The ad placement zone {0} is not ready.",
				zoneId == null ? "default" : zoneId));
		}

		#else
		Debug.LogError("Failed to show ad. Unity Ads does not support the current build platform.");
		#endif
	}

	#if UNITY_IOS || UNITY_ANDROID
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
	#endif

	private static void ClearActions ()
	{
		onFinished = null;
		onSkipped = null;
		onFailed = null;
	}

	private static string Validate (string value)
	{
		if (value != null) value = value.Trim();
		if (string.IsNullOrEmpty(value)) value = null;

		return value;
	}
}
