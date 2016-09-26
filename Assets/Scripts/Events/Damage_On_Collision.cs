using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Damage a component that is collided with.
 */

public class Damage_On_Collision : MonoBehaviour {

	public float Damage;
	public List<Transform> IgnoreCollisions = new List<Transform>();	// A list of collisions to ignore

	// Use this for initialization
	void Start () {
	
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
			collision.transform.ApplyDamage(Damage);
	}
}
