using UnityEngine;
using System.Collections;
using System;

public class UnityAdsExample : MonoBehaviour 
{
	public static Action onResetCooldownEvent;

	void Start ()
	{
		if (Debug.isDebugBuild)
		{
			OnResetCooldown();
		}

		UnityAdsHelper.Initialize();
	}

	public void OnResetCooldown ()
	{
		if (onResetCooldownEvent != null) 
		{
			Debug.Log("Resetting cooldowns...");
			onResetCooldownEvent();
		}
	}
}
