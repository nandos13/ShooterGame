using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Disable a component when it collides with another object.
 */

public class Disable_On_Collision : MonoBehaviour {

	private Rigidbody rb;
	public List<Transform> CollisionList = new List<Transform>();	// A list of collisions to ignore
	public COLLISION_MODE mode = COLLISION_MODE.IgnoreSelected;

	void OnCollisionEnter (Collision collision)
	{
		// Should this collision be ignored?
		bool ignoring = false;
		if (mode == COLLISION_MODE.HitSelected)
			ignoring = true;

		foreach (Transform t in CollisionList)
		{
			if (collision.transform == t)
			{
				if (mode == COLLISION_MODE.IgnoreSelected)
				{
					ignoring = true;
					break;
				}
				else if (mode == COLLISION_MODE.HitSelected)
				{
					ignoring = false;
					break;
				}
			}
		}

		if (!ignoring)
			gameObject.SetActive (false);
	}
}
