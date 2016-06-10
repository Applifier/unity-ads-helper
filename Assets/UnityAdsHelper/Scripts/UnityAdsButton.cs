using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DateTime = System.DateTime;

[RequireComponent(typeof(Button))]
public class UnityAdsButton : MonoBehaviour 
{
	public bool rewarded;
	public string placementId;
	public float cooldownTime;

	public UnityEvent onFinished;
	public UnityEvent onSkipped;
	public UnityEvent onFailed;

	private string _keyCooldownTime = "CooldownTime";
	private DateTime _rewardCooldownTime;

	private Button _button;

	void Start ()
	{
		_button = GetComponent<Button>();

		if (_button) _button.onClick.AddListener(ShowAd);

		_keyCooldownTime += name + gameObject.GetInstanceID().ToString();
		_rewardCooldownTime = GetCooldownTime();

		if (!UnityAdsHelper.isInitialized) UnityAdsHelper.Initialize();
	}

	void Update ()
	{
		if (_button) _button.interactable = IsReady();
	}

	private bool IsReady ()
	{
		if (DateTime.Compare(DateTime.UtcNow,_rewardCooldownTime) > 0)
		{
			return UnityAdsHelper.IsReady(placementId);
		}
		else return false;
	}

	public void ShowAd ()
	{
		if (onFinished != null) UnityAdsHelper.onFinished = delegate 
		{ 
			if (cooldownTime > 0)
			{
				SetCooldownTime(DateTime.UtcNow.AddSeconds(cooldownTime));
				Debug.Log(string.Format("Cooldown timer set. " +
					"Next ad available in {0} seconds.",cooldownTime));
			}

			onFinished.Invoke(); 
		};
		if (onSkipped != null) UnityAdsHelper.onSkipped = delegate { onSkipped.Invoke(); };
		if (onFailed != null) UnityAdsHelper.onFailed = delegate { onFailed.Invoke(); };

		UnityAdsHelper.ShowAd(placementId,rewarded);
	}

	private DateTime GetCooldownTime ()
	{
		if (object.Equals(_rewardCooldownTime,default(DateTime)))
		{
			if (PlayerPrefs.HasKey(_keyCooldownTime))
			{
				_rewardCooldownTime = DateTime.Parse(PlayerPrefs.GetString(_keyCooldownTime));

				if (Debug.isDebugBuild)
				{
					DateTime appStartTime = DateTime.UtcNow.AddSeconds(-1*Time.time);
					DateTime lastRewardTime = _rewardCooldownTime.AddSeconds(-1*cooldownTime);

					if (DateTime.Compare(appStartTime,lastRewardTime) > 0) ResetCooldownTime();
				}
			}
			else _rewardCooldownTime = DateTime.UtcNow;
		}

		return _rewardCooldownTime;
	}

	private void SetCooldownTime (DateTime dateTime)
	{
		_rewardCooldownTime = dateTime;
		PlayerPrefs.SetString(_keyCooldownTime,dateTime.ToString());
	}

	private void ResetCooldownTime ()
	{
		Debug.Log("Cooldown timer reset for " + name + ".");
		SetCooldownTime(DateTime.UtcNow);
	}
}
