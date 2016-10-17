using UnityEngine;
using System.Collections;

public class CrystalRotation : MonoBehaviour {

	public float value = 1;
	public bool x = true;
	public bool y = true;
	public bool z = true;

	void Start () 
	{
		
	}

	void Update () 
	{
		if (!Options.Paused)
		{
			if (x)
				transform.Rotate (transform.right, value);
			if (y)
				transform.Rotate (transform.up, value);
			if (z)
				transform.Rotate (transform.forward, value);
		}
	}
}
