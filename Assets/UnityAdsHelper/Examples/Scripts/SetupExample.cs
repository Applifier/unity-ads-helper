#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_LEGACY
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if !UNITY_LEGACY
using UnityEngine.SceneManagement;
#endif

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
#if !UNITY_LEGACY
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
#else
		Application.LoadLevel(Application.loadedLevel + 1);
#endif
	}
}
