using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Rotates a transform to always look at the camera (used for UI billboards, etc)
 */

public class FacePlayer : MonoBehaviour {

	void Update () 
	{
		// Rotate the canvas to always face the player
		Camera mainCam = Camera.main;
		Vector3 angleToPlayer = transform.position - mainCam.transform.position;
		transform.rotation = Quaternion.LookRotation (angleToPlayer, Vector3.up);
	}
}
