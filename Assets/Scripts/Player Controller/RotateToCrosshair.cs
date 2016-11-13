using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Rotates a transform to face the surface being aimed at by the player.
 * If no surface is being aimed at, will point 1000 units away from the player
 * in the correct direction.
 */

public class RotateToCrosshair : MonoBehaviour {

	private Ray ray;
	private RaycastHit[] hits;
	private RaycastHit hit = new RaycastHit ();
	private string[] tags;

	void Start ()
	{
		tags = new string[1];
		tags[0] = "Player";
	}

	void Update () 
	{
		// Raycast forward from the center of the camera
		ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0.0f));
		hits = Physics.RaycastAll (ray, 1000.0f);
		hit = new RaycastHit ();

		hit = hits.ApplyTagMask (tags);

		//// Find the first hit that is not part of the player
		//bool aimingCollides = false;
		//foreach (RaycastHit h in hits)
		//{
		//	// Ignore player
		//	if ( !(h.collider.tag == "Player") )
		//	{
		//		aimingCollides = true;
		//		hit = h;
		//		break;
		//	}
		//}

		// Is the raycast aiming at something?
		if (hit.collider)
		{
			transform.LookAt (hit.point);
		}
		else
		{
			// Point 1000 units away from the center of the screen
			Vector3 lookPoint = ray.GetPoint(1000);
			transform.LookAt (lookPoint);
		}
	}
}
