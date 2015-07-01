using UnityEngine;
using System.Collections;

public class ShowAdOnLoad : MonoBehaviour 
{
	/// <summary>
	/// The Unity Ads zone identifier. Leave empty or assign a null value to show the default ad placement.
	/// </summary>
	[SerializeField]
	private string _zoneId;
	/// <summary>
	/// Enable timeouts to cancel the show process if Unity Ads is not initialized or ready to show in a timely manner.
	/// </summary>
	[SerializeField]
	private bool _enableTimeouts = true;
	/// <summary>
	/// The maximum time in seconds allowed for Unity Ads to become initialized.
	/// </summary>
	[SerializeField]
	private float _initTimeout = 15f;
	/// <summary>
	/// The maximum time in seconds allowed for Unity Ads to be ready to show.
	/// </summary>
	[SerializeField]
	private float _showTimeout = 15f;
	/// <summary>
	/// The frequency in seconds for evaluating Unity Ads as initialized and ready to show.
	/// </summary>
	[SerializeField]
	private float _yieldTime = 0.5f;
	
	/// <summary>
	/// The start time since level load of the current timeout.
	/// </summary>
	private float _startTime = 0f;
	
#if UNITY_IOS || UNITY_ANDROID
	IEnumerator Start ()
	{
		string zoneName = string.IsNullOrEmpty(_zoneId) ? "the default ad placement zone" : _zoneId;
		
		_startTime = Time.timeSinceLevelLoad;
		
		while (!UnityAdsHelper.isInitialized)
		{
			if (_enableTimeouts && Time.timeSinceLevelLoad - _startTime > _initTimeout)
			{
				Debug.LogWarning(string.Format("Unity Ads failed to initialize in a timely manner. " +
				                               "An ad for {0} will not be shown on load.",zoneName));
				
				yield break;
			}

			yield return new WaitForSeconds(_yieldTime);
		}

		Debug.Log("Unity Ads has finished initializing. Waiting for ads to be ready...");
		
		_startTime = Time.timeSinceLevelLoad;
		
		while (!UnityAdsHelper.IsReady(_zoneId))
		{
			if (_enableTimeouts && Time.timeSinceLevelLoad - _startTime > _showTimeout)
			{
				Debug.LogWarning(string.Format("Unity Ads failed to be ready in a timely manner. " +
				                               "An ad for {0} will not be shown on load.",zoneName));
				
				yield break;
			}
			
			yield return new WaitForSeconds(_yieldTime);
		}
		
		Debug.Log(string.Format("Ads for {0} are available and ready. Showing ad now...",zoneName));
		
		UnityAdsHelper.ShowAd(_zoneId);
	}
#endif
}
