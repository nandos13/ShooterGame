using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * 
 */

[RequireComponent(typeof(Weapon_Base_Script))]
public class FPC_Weapon : MonoBehaviour 
{

	private Weapon_Base_Script weapon;
	private bool weaponUp = false;			// Track if the weapon is up while against a wall

	// Use this for initialization
	void Start () 
	{
		weapon = GetComponent<Weapon_Base_Script> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (weapon)
		{
			if (weapon.shotOrigin)
			{
				// Should the weapon be up?
				// Raycast forward from the center of the camera
				//TODO: ALSO CHECK IF THE SHOTORIGIN IS INSIDE AN OBJECT (MAYBE RAYCAST IN A DIRECTION, CHECK IF IN BOUNDS OF HIT?)
				Ray ray = Camera.main.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0.0f));
				RaycastHit[] hits = Physics.RaycastAll (ray, 1000.0f);
				RaycastHit hit = new RaycastHit();

				// Find the first hit that is not part of the player
				foreach (RaycastHit h in hits)
				{
					// Ignore player
					if ( !(h.collider.tag == "Player") )
					{
						hit = h;
						break;
					}
				}

				if (hit.collider)
				{
					if (Vector3.Distance(hit.point, transform.position) < Vector3.Distance(transform.position, weapon.shotOrigin.position))
					{
						// Wall in the way
						if (!weaponUp)
						{
							//TODO: ANIMATE WEAPON GOING UP
							weaponUp = true;
						}
					}
					else
					{
						// Nothing in the way
						if (weaponUp)
						{
							//TODO: ANIMATE WEAPON GOING DOWN
							weaponUp = false;
						}
					}
				}

				// Can the player fire?
				if (!weaponUp)
				{
					// Has the player tried to fire?
					if (Input.GetAxisRaw ("Fire1") > 0) 
					{
						weapon.Execute ();
					}
				}
			}
		}
	}
}
