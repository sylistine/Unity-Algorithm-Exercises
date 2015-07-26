using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseButton : MonoBehaviour
{
	Text buttonText;

	void Start ()
	{
		buttonText = this.GetComponentInChildren<Text>();
	}

	public void Pause ()
	{
		if (Time.timeScale == 0)
		{
			Time.timeScale = 1f;
			buttonText.text = "Pause";
		}
		else
		{
			Time.timeScale = 0;
			buttonText.text = "Play";
		}
	}
}
