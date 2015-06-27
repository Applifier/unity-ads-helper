using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class UnityAdsSettingsEditor : Editor 
{
	private const string settingsFile = "UnityAdsSettings";
	private const string settingsFileExtension = ".asset";

	[MenuItem("Edit/Unity Ads Settings")]
	public static void ShowSettings()
	{
		UnityAdsSettings settings = LoadUnityAdsSettings();
		
		if (settings == null) settings = CreateUnityAdsSettings();

		if (settings != null) Selection.activeObject = settings;
	}

	private static UnityAdsSettings LoadUnityAdsSettings ()
	{
		return (UnityAdsSettings)Resources.Load(settingsFile);
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
			
			AssetDatabase.CreateAsset(settings, "Assets/Resources/" + settingsFile + settingsFileExtension);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		return settings;
	}
}
