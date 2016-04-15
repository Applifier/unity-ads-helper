using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UnityAdsButton : MonoBehaviour 
{
	public string zoneId;
	public bool rewarded;

	public UnityEvent onFinished;
	public UnityEvent onSkipped;
	public UnityEvent onFailed;

	private Button _button;

	void Start ()
	{
		_button = GetComponent<Button>();

		if (_button) _button.onClick.AddListener (delegate() { ShowAd(); });
	}

	void Update ()
	{
		if (_button) _button.interactable = UnityAdsHelper.IsReady(zoneId);
	}

	private void ShowAd ()
	{
		if (onFinished != null) UnityAdsHelper.onFinished = delegate () { onFinished.Invoke(); };
		if (onSkipped != null) UnityAdsHelper.onSkipped = delegate () { onSkipped.Invoke(); };
		if (onFailed != null) UnityAdsHelper.onFailed = delegate () { onFailed.Invoke(); };

		if (rewarded) {
			UnityAdsHelper.ShowRewardedAd(zoneId);
		} else {
			UnityAdsHelper.ShowAd(zoneId);
		}
	}
}
