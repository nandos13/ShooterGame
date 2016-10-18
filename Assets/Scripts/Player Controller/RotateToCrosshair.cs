using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Rotates a transform to face the surface being aimed at by the player.
 * If no surface is being aimed at, will point 1000 units away from the player
 * in the correct direction.
 */

public class RotateToCrosshair : MonoBehaviour {

	void Update () 
	{
		// Raycast forward from the center of the camera
		Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0.0f));
		RaycastHit[] hits = Physics.RaycastAll (ray, 1000.0f);
		RaycastHit hit = new RaycastHit();

		// Find the first hit that is not part of the player
		bool aimingCollides = false;
		foreach (RaycastHit h in hits)
		{
			// Ignore player
			if ( !(h.collider.tag == "Player") )
			{
				aimingCollides = true;
				hit = h;
				break;
			}
		}

		// Is the raycast aiming at something?
		if (aimingCollides)
		{
			transform.LookAt (hit.point);
		}
		else
		{
			// Point 1000 units away from the center of the screen
			Vector3 lookPoint = (1000 * ray.direction) + transform.position;
			transform.LookAt (lookPoint);
		}
	}
}
