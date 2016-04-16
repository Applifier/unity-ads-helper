using UnityEngine;
using System.Collections;

public class UnityAdsSettings : ScriptableObject 
{
	public const string defaultIosGameId = "18660";
	public const string defaultAndroidGameId = "18658";

	public string iosGameId     = null;
	public string androidGameId = null;

	public bool overrideAdsServiceInit = true;

	public bool enableTestMode  = true;
	public bool showInfoLogs    = false;
	public bool showDebugLogs   = false;
	public bool showWarningLogs = true;
	public bool showErrorLogs   = true;

	public UnityAdsSettings ()
	{
		iosGameId = defaultIosGameId;
		androidGameId = defaultAndroidGameId;
	}
}
