using UnityEngine;
using System.Collections;

public class SpeedSlider : MonoBehaviour
{
	Percolation _percolation;

	public Percolation percolation
	{
		get
		{
			if(_percolation == null) _percolation = GameObject.Find ("Percolator(Clone)").GetComponent<Percolation>();
			return _percolation;
		}
	}
	public void setSpeed (float value)
	{
		percolation.updateSpeed = 1 / value;
	}
}
