using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Disable a component when it collides with another object.
 */

public class Disable_On_Collision : MonoBehaviour {

	private Rigidbody rb;
	public List<Transform> IgnoreCollisions = new List<Transform>();	// A list of collisions to ignore

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter (Collision collision)
	{
		// Should this collision be ignored?
		bool ignoring = false;
		foreach (Transform t in IgnoreCollisions)
		{
			if (collision.transform == t)
			{
				ignoring = true;
				break;
			}
		}

		if (!ignoring)
			gameObject.SetActive (false);
	}
}
