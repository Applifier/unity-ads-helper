using UnityEngine;
using System.Collections;

public class UnityAdsOnLoad : MonoBehaviour 
{
	public string placementId;

	public float initTimeout = 15f;  // Time in seconds to allow for init to complete before canceling show.
	public float showTimeout = 15f;  // Time in seconds to allow for ad to be ready before canceling show.

	private float _yieldTime = 0.5f; // Time in seconds between evaluation attempts.
	private float _startTime = 0f;

	IEnumerator Start ()
	{
		if (!UnityAdsHelper.isSupported) yield break;
		else if (!UnityAdsHelper.isInitialized) UnityAdsHelper.Initialize();

		string placementName = string.IsNullOrEmpty(placementId) ? "the default ad placement" : placementId;

		_startTime = Time.timeSinceLevelLoad;

		while (!UnityAdsHelper.isInitialized)
		{
			if (initTimeout > 0 && Time.timeSinceLevelLoad - _startTime > initTimeout)
			{
				Debug.LogWarning(string.Format("Unity Ads failed to initialize in a timely manner. " +
					"An ad for {0} will not be shown on load.",placementName));
				yield break;
			}

			yield return new WaitForSeconds(_yieldTime);
		}

		Debug.Log("Unity Ads has finished initializing. Waiting for ads to be ready...");

		_startTime = Time.timeSinceLevelLoad;

		while (!UnityAdsHelper.IsReady(placementId))
		{
			if (showTimeout > 0 && Time.timeSinceLevelLoad - _startTime > showTimeout)
			{
				Debug.LogWarning(string.Format("Unity Ads failed to be ready in a timely manner. " +
					"An ad for {0} will not be shown on load.",placementName));
				yield break;
			}

			yield return new WaitForSeconds(_yieldTime);
		}

		Debug.Log(string.Format("Ads for {0} are available and ready.",placementName));

		UnityAdsHelper.ShowAd(placementId);
	}

	void OnDestroy ()
	{
		StopAllCoroutines();
	}
}
