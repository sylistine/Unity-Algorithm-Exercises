using UnityEngine;
using System.Collections;

public class SpeedSlider : MonoBehaviour
{
	Percolation2D _percolation;

	public Percolation2D percolation
	{
		get
		{
			if(_percolation == null) _percolation = GameObject.Find ("Percolator(Clone)").GetComponent<Percolation2D>();
			return _percolation;
		}
	}
	public void setSpeed (float value)
	{
		percolation.updateSpeed = 1 / value;
	}
}
