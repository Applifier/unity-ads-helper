using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_ADS
using UnityEditor.Advertisements;
#endif

[CustomEditor(typeof(UnityAdsSettings))]
public class UnityAdsSettingsEditor : Editor 
{
	private const string _settingsFile = "UnityAdsSettings";
	private const string _settingsFileExtension = ".asset";

	private const string _helpMsgPlatform = "Platform must be set to either iOS or Android " +
		"for Unity Ads to be initialized in editor.";
	private const string _helpMsgGameIds = "Enter your game IDs for iOS and Android into the fields below. " +
		"Game IDs can be found listed on the Games page of the Unity Ads Admin.";
	private const string _helpMsgTestMode = "Test mode should be enabled while testing the functionality of Unity Ads. " +
		"To use test mode, Development Build must also be enabled.";
	private const string _helpMsgLogLevels = "Customize the level of debugging by enabling or disabling " +
		"the following log levels.";

	private const string _urlUnityAdsDocs  = "http://github.com/Applifier/unity-ads-helper/wiki";
	private const string _urlUnityAdsAdmin = "http://dashboard.unityads.unity3d.com";
	private const string _urlUnityAdsForum = "http://forum.unity3d.com/forums/unity-ads.67/";
	
	[MenuItem("Edit/Unity Ads Settings")]
	public static void ShowSettings()
	{
		UnityAdsSettings settings = LoadUnityAdsSettings();
		
		if (settings == null) settings = CreateUnityAdsSettings();

		if (settings != null) Selection.activeObject = settings;
	}

	[PostProcessSceneAttribute]
	public static void OnPostProcessScene ()
	{
		UnityAdsSettings settings = LoadUnityAdsSettings();

		if (settings == null) settings = CreateUnityAdsSettings();
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Admin")) Help.BrowseURL(_urlUnityAdsAdmin);
		if (GUILayout.Button("Documentation")) Help.BrowseURL(_urlUnityAdsDocs);
		if (GUILayout.Button("Forum")) Help.BrowseURL(_urlUnityAdsForum);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		bool overrideInitialization = false;

		#if UNITY_ADS
		overrideInitialization = !AdvertisementSettings.initializeOnStartup;

		overrideInitialization = 
			EditorGUILayout.ToggleLeft(" Override initialization of Unity Ads",overrideInitialization);

		AdvertisementSettings.initializeOnStartup = !overrideInitialization;

		EditorGUILayout.Space();
		#endif

		GUI.enabled = !Application.isPlaying;

		UnityAdsSettings settings = (UnityAdsSettings)target;

		Undo.RecordObject(settings,"Inspector");

		MessageType _msgTypeGameIds = MessageType.Info;
		#if UNITY_IOS
		if (settings.iosGameId == UnityAdsSettings.defaultIosGameId)
		{
			_msgTypeGameIds = MessageType.Warning;
		}
		else if (string.IsNullOrEmpty(settings.iosGameId.Trim()))
		{
			_msgTypeGameIds = MessageType.Error;
		} 
		#elif UNITY_ANDROID
		if (settings.androidGameId == UnityAdsSettings.defaultAndroidGameId)
		{
			_msgTypeGameIds = MessageType.Warning;
		}
		else if (string.IsNullOrEmpty(settings.androidGameId.Trim()))
		{
			_msgTypeGameIds = MessageType.Error;
		}
		#else
		EditorGUILayout.HelpBox(_helpMsgPlatform,MessageType.Warning);
		#endif

		if (overrideInitialization) 
		{
			EditorGUILayout.HelpBox(_helpMsgGameIds, _msgTypeGameIds);
			settings.iosGameId = EditorGUILayout.TextField("iOS Game ID", settings.iosGameId);
			settings.androidGameId = EditorGUILayout.TextField("Android Game ID", settings.androidGameId);

			EditorGUILayout.Space();

			MessageType _msgTypeTestMode = MessageType.Info;
			if (settings.enableTestMode && !EditorUserBuildSettings.development) {
				_msgTypeTestMode = MessageType.Warning;
			}

			EditorGUILayout.HelpBox(_helpMsgTestMode, _msgTypeTestMode);
			settings.enableTestMode = EditorGUILayout.ToggleLeft(" Enable Test Mode", settings.enableTestMode);
			EditorUserBuildSettings.development = 
				EditorGUILayout.ToggleLeft(" Enable Development Build", EditorUserBuildSettings.development);

			EditorGUILayout.Space();
		}

		EditorGUILayout.HelpBox(_helpMsgLogLevels,MessageType.Info);
		settings.showInfoLogs    = EditorGUILayout.ToggleLeft(" Show Info Logs",settings.showInfoLogs);
		settings.showDebugLogs   = EditorGUILayout.ToggleLeft(" Show Debug Logs",settings.showDebugLogs);
		settings.showWarningLogs = EditorGUILayout.ToggleLeft(" Show Warning Logs",settings.showWarningLogs);
		settings.showErrorLogs   = EditorGUILayout.ToggleLeft(" Show Error Logs",settings.showErrorLogs);

		EditorUtility.SetDirty(settings);
		EditorApplication.SaveAssets();

		EditorGUILayout.Space();

		GUI.enabled = true;
	}

	private static UnityAdsSettings LoadUnityAdsSettings ()
	{
		return (UnityAdsSettings)Resources.Load(_settingsFile);
	}
	
	private static UnityAdsSettings CreateUnityAdsSettings ()
	{
		UnityAdsSettings settings = (UnityAdsSettings)ScriptableObject.CreateInstance(typeof(UnityAdsSettings));
		
		if (settings != null) 
		{
			if (!System.IO.Directory.Exists(Application.dataPath + "/Resources"))
			{
				AssetDatabase.CreateFolder("Assets","Resources");
			}

			AssetDatabase.CreateAsset(settings, "Assets/Resources/" + _settingsFile + _settingsFileExtension);

			#if UNITY_ADS
			settings.androidGameId = AdvertisementSettings.GetGameId(RuntimePlatform.Android);
			settings.iosGameId = AdvertisementSettings.GetGameId(RuntimePlatform.IPhonePlayer);
			#endif

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		return settings;
	}
}
