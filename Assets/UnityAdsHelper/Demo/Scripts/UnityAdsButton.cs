using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent (typeof (Button))]
public class UnityAdsButton : MonoBehaviour 
{
	const string keyRewardCooldownTime = "RewardCooldownTime";

	public string zoneID;
	
	public float rewardCooldown = 300f;
	public int rewardCoinAmount = 250;
	
	private Button _button;
	
	private static DateTime _rewardCooldownTime;
	
	void Start ()
	{
		_button = GetComponent<Button>();
		_button.interactable = false;
		
		_rewardCooldownTime = GetRewardCooldownTime();
	}
	
	void Update ()
	{
		_button.interactable = IsReady();
	}
	
	private bool IsReady ()
	{
		if (DateTime.Compare(DateTime.UtcNow,_rewardCooldownTime) > 0)
		{
			return UnityAdsHelper.IsReady(zoneID);
		}
		else return false;
	}
	
	public void ShowAd ()
	{
		UnityAdsHelper.onFinishedEvent = RewardUserAndUpdateCooldownTime;
		UnityAdsHelper.ShowAd(zoneID);
	}
	
	private void RewardUserAndUpdateCooldownTime ()
	{
		Debug.Log("Granting the user a reward...");
		
		//--- Example: Inventory.AddCoins(rewardCoinAmount);
		
		SetRewardCooldownTime(DateTime.UtcNow.AddSeconds(rewardCooldown));
		
		Debug.Log(string.Format("User was rewarded. Next rewarded ad is available in {0} seconds.",rewardCooldown));
	}
	
	//--- Reward Cooldown Methods
	
	public static DateTime GetRewardCooldownTime ()
	{
		if (object.Equals(_rewardCooldownTime,default(DateTime)))
		{
			if (PlayerPrefs.HasKey(keyRewardCooldownTime))
			{
				_rewardCooldownTime = DateTime.Parse(PlayerPrefs.GetString(keyRewardCooldownTime));
			}
			else _rewardCooldownTime = DateTime.UtcNow;
		}
		
		return _rewardCooldownTime;
	}
	
	public static void SetRewardCooldownTime (DateTime dateTime)
	{
		_rewardCooldownTime = dateTime;
		PlayerPrefs.SetString(keyRewardCooldownTime,dateTime.ToString());
	}
	
	public static void ResetRewardCooldownTime ()
	{
		Debug.Log("Reset reward cooldown time.");
		SetRewardCooldownTime(DateTime.UtcNow);
	}
}
