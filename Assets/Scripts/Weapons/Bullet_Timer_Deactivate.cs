using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Disables the object after specified time elapsed
 */

public class Bullet_Timer_Deactivate : MonoBehaviour {

	[Range(0.1f, 7.0f)]
	public float DisableAfterSeconds = 7.0f;

	// Called once the object is activated
	void OnEnable () 
	{
		StartCoroutine (DeactivateOnTimer(DisableAfterSeconds));
	}

	public IEnumerator DeactivateOnTimer(float x)
	{
		/* This function disables the projectile after x seconds,
		 * removing it from the game-world and returning 
		 * it to the pool.
		 */
		//Debug.Log ("Bullet timer started!");
		yield return new WaitForSeconds (x);
		//Debug.Log ("Bullet being deactivated");
		gameObject.SetActive (false);
		StopAllCoroutines ();
	}
}
