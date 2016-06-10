using UnityEngine;
using System.Collections;

public class CreditsButton : MonoBehaviour 
{
	public void OpenURL (string url)
	{
		Application.OpenURL(url);
	}
}
