using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnityAdsDemo : MonoBehaviour 
{
	void Start ()
	{
		if (Debug.isDebugBuild) UnityAdsButton.ResetRewardCooldownTime();

		UnityAdsHelper.Initialize();
	}
}
