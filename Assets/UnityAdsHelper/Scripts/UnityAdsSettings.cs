using UnityEngine;
using System.Collections;

public class UnityAdsSettings : ScriptableObject 
{
	public string iosGameId     = "18660";
	public string androidGameId = "18658";
	
	public bool enableTestMode  = true;

	public bool showInfoLogs    = false;
	public bool showDebugLogs   = false;
	public bool showWarningLogs = true;
	public bool showErrorLogs   = true;

	public bool initOnStart     = false;
}
