using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EventSystem = UnityEngine.EventSystems.EventSystem;
using System.Collections;

public class UnityAdsButtonEditor : Editor 
{
	[MenuItem("GameObject/Unity Ads/Button",false,5)]
	public static void CreateUnityAdsButton ()
	{
		EventSystem eventsys = GameObject.FindObjectOfType<EventSystem>();
		if (eventsys == null)
		{
			GameObject prefab = Resources.Load("EventSystem",typeof(GameObject)) as GameObject;
			if (prefab)
			{
				GameObject gO = Instantiate(prefab) as GameObject;
				if (gO)
				{ 
					gO.name = "EventSystem";
					eventsys = gO.GetComponent<EventSystem>();
				}
			}
		}

		Canvas canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas == null)
		{
			GameObject prefab = Resources.Load("Canvas",typeof(GameObject)) as GameObject;
			if (prefab)
			{
				GameObject gO = Instantiate(prefab) as GameObject;
				if (gO)
				{
					gO.name = "Canvas";
					canvas = gO.GetComponent<Canvas>();
				}
			}
		}
			
		Button button = GameObject.FindObjectOfType<Button>();
		if (button == null)
		{
			GameObject prefab = Resources.Load("Button",typeof(GameObject)) as GameObject;
			if (prefab)
			{
				GameObject gO = Instantiate(prefab) as GameObject;
				if (gO)
				{
					gO.name = "UnityAdsButton";
					gO.AddComponent<UnityAdsButton>();

					if (canvas) gO.GetComponent<Transform>().SetParent(canvas.transform);

					button = gO.GetComponent<Button>();
				}
			}
		}

	}
}
