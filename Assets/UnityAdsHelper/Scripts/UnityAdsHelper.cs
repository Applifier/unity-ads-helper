using System;
using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour 
{
	public static Action onFinishedEvent;
	public static Action onSkippedEvent;
	public static Action onFailedEvent;

	private static string _gamerSID;
	public static void SetGamerSID (string gamerSID)
	{
		_gamerSID = string.IsNullOrEmpty(gamerSID) ? null : gamerSID;
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
			if (_instance == null) gO.AddComponent<UnityAdsHelper>();
		}
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) _instance = this;
		else if (_instance != this) Destroy(gameObject);
	}
	
	public static void Initialize () 
	{ 
		if (_isInitializing) return;
		
		UnityAdsHelper instance = GetInstance();
		if (instance != null) instance.OnInitialize();
	}
	
	private void OnInitialize ()
	{
		_isInitializing = true;

		Debug.Log("Running precheck for Unity Ads initialization...");
		
		UnityAdsSettings settings = (UnityAdsSettings)Resources.Load("UnityAdsSettings");

		if (settings == null)
		{
			Debug.LogError("Failed to initialize Unity Ads. Settings file not found.");
			_isInitializing = false;
			return;
		}

		string gameId = null;
		
	#if UNITY_IOS
		gameId = settings.iosGameId;
	#elif UNITY_ANDROID
		gameId = settings.androidGameId;
	#endif
		
		if (!Advertisement.isSupported)
		{
			Debug.LogWarning("Unity Ads is not supported on the current runtime platform.");
		}
		else if (Advertisement.isInitialized)
		{
			Debug.LogWarning("Unity Ads is already initialized.");
		}
		else if (string.IsNullOrEmpty(gameId))
		{
			Debug.LogError("The game ID value is not set. A valid game ID is required to initialize Unity Ads.");
		}
		else
		{
			Advertisement.debugLevel = Advertisement.DebugLevel.None;

			if (settings.showInfoLogs)    Advertisement.debugLevel |= Advertisement.DebugLevel.Info;
			if (settings.showDebugLogs)   Advertisement.debugLevel |= Advertisement.DebugLevel.Debug;
			if (settings.showWarningLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.Warning;
			if (settings.showErrorLogs)   Advertisement.debugLevel |= Advertisement.DebugLevel.Error;
			
			if (settings.enableTestMode && !Debug.isDebugBuild)
			{
				Debug.LogWarning("Development Build must be enabled in Build Settings to enable test mode for Unity Ads.");
			}
			
			bool isTestModeEnabled = Debug.isDebugBuild && settings.enableTestMode;
			Debug.Log(string.Format("Precheck done. Initializing Unity Ads for game ID {0} with test mode {1}...",
			                        gameId, isTestModeEnabled ? "enabled" : "disabled"));
			
			Advertisement.Initialize(gameId,isTestModeEnabled);
			
			StartCoroutine(LogWhenUnityAdsIsInitialized());
		}
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
	
	public static bool isShowing { get { return Advertisement.isShowing; }}
	public static bool isSupported { get { return Advertisement.isSupported; }}
	public static bool isInitialized { get { return Advertisement.isInitialized; }}
	
	public static bool IsReady () { return IsReady(null); }
	public static bool IsReady (string zoneID) 
	{
		if (string.IsNullOrEmpty(zoneID)) zoneID = null;
		
		return Advertisement.IsReady(zoneID);
	}

	public static void ShowAd () { ShowAd(null); }
	public static void ShowAd (string zoneId)
	{
		if (string.IsNullOrEmpty(zoneId)) zoneId = null;
		
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
			if (onFinishedEvent != null) onFinishedEvent();
			break;
		case ShowResult.Skipped:
			Debug.LogWarning("The ad was skipped before reaching the end.");
			if (onSkippedEvent != null) onSkippedEvent();
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			if (onFailedEvent != null) onFailedEvent();
			break;
		}
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
	public static bool IsReady (string zoneID) { return false; }
	
	public static void ShowAd () { ShowAd(null); }
	public static void ShowAd (string zoneID) 
	{
		Debug.LogError("Failed to show ad. Unity Ads does not support the current build platform.");
	}

#endif
}
