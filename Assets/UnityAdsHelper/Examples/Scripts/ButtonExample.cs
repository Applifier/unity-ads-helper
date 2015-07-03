using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ButtonExample : MonoBehaviour 
{
	public Text textReady;
	public Text textWaiting;

	public string zoneId;

	public float cooldownTime = 300f;
	public int rewardAmount = 250;

	private string _keyCooldownTime = "CooldownTime";
	private DateTime _rewardCooldownTime;

	private Button _button;

	void Awake ()
	{
		_button = GetComponent<Button>();

		_keyCooldownTime += name + gameObject.GetInstanceID().ToString();
		_rewardCooldownTime = GetCooldownTime();

		UnityAdsExample.onResetCooldownEvent += ResetCooldownTime;
	}

	void Update ()
	{
		if (_button)
		{
			_button.interactable = IsReady();

			if (textReady) textReady.enabled = _button.interactable;
			if (textWaiting) textWaiting.enabled = !_button.interactable;
		}
	}

	void OnDestroy ()
	{
		UnityAdsExample.onResetCooldownEvent -= ResetCooldownTime;
	}

	private bool IsReady ()
	{
		if (DateTime.Compare(DateTime.UtcNow,_rewardCooldownTime) > 0)
		{
			return UnityAdsHelper.IsReady(zoneId);
		}
		else return false;
	}

	public void ShowAd ()
	{
		UnityAdsHelper.onFinishedEvent = OnFinished;
		UnityAdsHelper.ShowAd(zoneId);
	}

	private void OnFinished ()
	{
		if (rewardAmount > 0) 
		{
			Debug.Log("The player has earned a reward!");
		}

		if (cooldownTime > 0)
		{
			SetCooldownTime(DateTime.UtcNow.AddSeconds(cooldownTime));
			Debug.Log(string.Format("Next ad is available in {0} seconds.",cooldownTime));
		}
	}
	
	private DateTime GetCooldownTime ()
	{
		if (object.Equals(_rewardCooldownTime,default(DateTime)))
		{
			if (PlayerPrefs.HasKey(_keyCooldownTime))
			{
				_rewardCooldownTime = DateTime.Parse(PlayerPrefs.GetString(_keyCooldownTime));
			}
			else _rewardCooldownTime = DateTime.UtcNow;
		}
		
		return _rewardCooldownTime;
	}
	
	private void SetCooldownTime (DateTime dateTime)
	{
		_rewardCooldownTime = dateTime;
		PlayerPrefs.SetString(_keyCooldownTime,dateTime.ToString());
		Debug.Log(_keyCooldownTime);
	}
	
	private void ResetCooldownTime ()
	{
		Debug.Log(string.Format("Cooldown time reset for: {0} ({1})",name,gameObject.GetInstanceID().ToString()));
		SetCooldownTime(DateTime.UtcNow);
	}
}
