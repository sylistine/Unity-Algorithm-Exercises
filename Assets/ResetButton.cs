using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResetButton : MonoBehaviour
{
	public GameObject percolator;

	Text buttonText;

	void Start ()
	{
		buttonText = this.GetComponentInChildren<Text> ();
		buttonText.text = "Start";
	}

	public void Reset ()
	{
		buttonText.text = "Reset";
		GameObject.Destroy (GameObject.Find ("Percolator(Clone)"));
		Instantiate(percolator);
	}
}
