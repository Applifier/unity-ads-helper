using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using System.IO;

[CustomEditor(typeof(UnityAdsSettings))]
public class UnityAdsSettingsEditor : Editor 
{
	private const string _settingsFile = "UnityAdsSettings";
	private const string _settingsFileExtension = ".asset";

	private const string _helpMsgGameIds = "Enter your game IDs for iOS and Android into the fields below. " +
		"Game IDs can be found listed on the Games page of the Unity Ads Admin.";
	private const string _helpMsgTestMode = "Test mode should be enabled while testing the functionality of Unity Ads. " +
		"To use test mode, Development Build must also be enabled.";
	private const string _helpMsgLogLevels = "Customize the level of debugging by enabling or disabling " +
		"the following log levels.";

	private const string _urlUnityAdsAdmin = "http://unityads.unity3d.com/admin";
	private const string _urlUnityAdsDocs = "http://unityads.unity3d.com/help";
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

		GUI.enabled = !Application.isPlaying;

		UnityAdsSettings settings = (UnityAdsSettings)target;

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
	#endif

		EditorGUILayout.HelpBox(_helpMsgGameIds,_msgTypeGameIds);
		settings.iosGameId       = EditorGUILayout.TextField("iOS Game ID",settings.iosGameId);
		settings.androidGameId   = EditorGUILayout.TextField("Android Game ID",settings.androidGameId);

		EditorGUILayout.Space();

		MessageType _msgTypeTestMode = MessageType.Info;
		if (settings.enableTestMode && !EditorUserBuildSettings.development) 
		{
			_msgTypeTestMode = MessageType.Warning;
		}

		EditorGUILayout.HelpBox(_helpMsgTestMode,_msgTypeTestMode);
		settings.enableTestMode  = EditorGUILayout.ToggleLeft(" Enable Test Mode",settings.enableTestMode);

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox(_helpMsgLogLevels,MessageType.Info);
		settings.showInfoLogs    = EditorGUILayout.ToggleLeft(" Show Info Logs",settings.showInfoLogs);
		settings.showDebugLogs   = EditorGUILayout.ToggleLeft(" Show Debug Logs",settings.showDebugLogs);
		settings.showWarningLogs = EditorGUILayout.ToggleLeft(" Show Warning Logs",settings.showWarningLogs);
		settings.showErrorLogs   = EditorGUILayout.ToggleLeft(" Show Error Logs",settings.showErrorLogs);

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
			if(!AssetDatabase.IsValidFolder("Assets/Resources")) 
			{
				AssetDatabase.CreateFolder("Assets","Resources");
			}
			
			AssetDatabase.CreateAsset(settings, "Assets/Resources/" + _settingsFile + _settingsFileExtension);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		return settings;
	}
}
