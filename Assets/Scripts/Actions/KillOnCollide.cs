using UnityEngine;
using System.Collections;

public class KillOnCollide : MonoBehaviour {

	void OnCollisionEnter (Collision col)
	{
		// Find a health script on the colliding object
		Health tempHealth = col.gameObject.GetComponent<Health>();

		if (tempHealth)
		{
			// Kill it
			tempHealth.Alive = false;
		}
	}
}
