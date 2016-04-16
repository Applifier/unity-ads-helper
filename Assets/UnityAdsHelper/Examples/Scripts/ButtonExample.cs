using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Button))]
public class ButtonExample : MonoBehaviour 
{
	public Text readyText;
	public Text waitingText;

	private Button _button;

	void Awake ()
	{
		_button = GetComponent<Button>();
	}

	void Update ()
	{
		if (_button)
		{
			if (readyText) readyText.enabled = _button.interactable;
			if (waitingText) waitingText.enabled = !_button.interactable;
		}
	}
}
