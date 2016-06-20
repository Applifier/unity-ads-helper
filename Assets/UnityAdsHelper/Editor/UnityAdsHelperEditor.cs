using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EventSystem = UnityEngine.EventSystems.EventSystem;
using System.Collections;

public class UnityAdsHelperEditor : Editor 
{
	[MenuItem("GameObject/Unity Ads/Helper",false,5)]
	public static void CreteUnityAdsHelper ()
	{
		UnityAdsHelper helper = GameObject.FindObjectOfType<UnityAdsHelper>() as UnityAdsHelper;
		GameObject gO = (helper != null ? helper.gameObject : null);

		if (gO == null)
		{
			gO = new GameObject("UnityAdsHelper");
			gO.AddComponent<UnityAdsHelper>();
		}

		if (gO) Selection.activeGameObject = gO;
	}

	[MenuItem("GameObject/Unity Ads/Button",false,5)]
	public static void CreateUnityAdsButton ()
	{
		EventSystem eventsys = GameObject.FindObjectOfType<EventSystem>();
		if (eventsys == null)
		{
			GameObject gO = new GameObject("EventSystem");
			eventsys = gO.AddComponent<EventSystem>();
			gO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
		}

		//TODO: Get canvas in closest relation to current selected object.
		Canvas canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas == null)
		{
			GameObject gO = new GameObject("Canvas", typeof(RectTransform));
			canvas = gO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			gO.AddComponent<CanvasScaler>();
			gO.AddComponent<GraphicRaycaster>();
			gO.layer = 5;
		}
			
		GameObject buttonObj = new GameObject("UnityAdsButton", typeof(RectTransform));

		if (canvas) buttonObj.transform.SetParent(canvas.transform);

		RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
		buttonRect.anchoredPosition = Vector3.zero;
		buttonRect.sizeDelta = new Vector2(160f,30f);
		buttonObj.AddComponent<Image>();
		buttonObj.AddComponent<UnityAdsButton>();
		buttonObj.layer = 5;

		GameObject textObj = new GameObject("Text", typeof(RectTransform));

		if (buttonObj) textObj.transform.SetParent(buttonObj.transform);

		RectTransform textRect = textObj.GetComponent<RectTransform>();
		textRect.anchoredPosition = Vector3.zero;
		textRect.anchorMin = new Vector2(0f, 0f);
		textRect.anchorMax = new Vector2(1f, 1f);
		textObj.AddComponent<Text>();

		textObj.layer = 5;

		Selection.activeGameObject = buttonObj;
	}
}
