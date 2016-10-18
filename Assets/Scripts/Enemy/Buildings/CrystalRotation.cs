using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Rotates an object around specified axis by a specified value each frame.
 */

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
				transform.Rotate (transform.right, value * Time.deltaTime);
			if (y)
				transform.Rotate (transform.up, value * Time.deltaTime);
			if (z)
				transform.Rotate (transform.forward, value * Time.deltaTime);
		}
	}
}
