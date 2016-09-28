using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * 
 */

[RequireComponent(typeof(Weapon_Base_Script))]
public class FPC_Weapon : MonoBehaviour 
{

	public LayerMask layers;
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
				RaycastHit hit = new RaycastHit();
				Physics.Linecast (weapon.shotOrigin.position, transform.position, out hit, layers);

				if (hit.collider)
				{
					// Wall in the way
					if (!weaponUp)
					{
						//TODO: ANIMATE WEAPON GOING UP
						weaponUp = true;
					}
					Debug.Log(hit.transform.name);
				}
				else
				{
					//TODO: ANIMATE WEAPON GOING DOWN
					weaponUp = false;
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
