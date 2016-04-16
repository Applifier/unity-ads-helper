using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SetupExample : MonoBehaviour 
{
	public InputField gameIdField;
	public Toggle testModeToggle;
	public Button submitButton;

	void Start ()
	{
		if (UnityAdsHelper.Settings == null) return;

		string gameId = null;
		bool enableTestMode = false;

		#if UNITY_IOS
		gameId = UnityAdsHelper.Settings.iosGameId.Trim();
		#elif UNITY_ANDROID
		gameId = UnityAdsHelper.Settings.androidGameId.Trim();
		#endif

		enableTestMode = UnityAdsHelper.Settings.enableTestMode;

		if (gameIdField) gameIdField.text = gameId;
		if (testModeToggle) testModeToggle.isOn = enableTestMode;
	}

	void Update ()
	{
		if (gameIdField && submitButton) 
		{
			submitButton.interactable = !string.IsNullOrEmpty(gameIdField.text);
		}
	}

	public void Submit ()
	{
		string gameId = null;
		bool enableTestMode = false;

		if (gameIdField) gameId = gameIdField.text;
		if (testModeToggle) enableTestMode = testModeToggle.isOn;

		UnityAdsHelper.Initialize(gameId,enableTestMode);
		SceneManager.LoadScene("UnityAdsExample");
	}
}
