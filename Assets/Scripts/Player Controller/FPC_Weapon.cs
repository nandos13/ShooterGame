using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * 
 */

public class FPC_Weapon : MonoBehaviour 
{

	public LayerMask layers;
	public List<WeaponBase> Guns = new List<WeaponBase>();		// A list of all the weapons held by the player
	private WeaponBase currentWeapon;
	private bool weaponUp = false;								// Track if the weapon is up while against a wall

	// Use this for initialization
	void Start () 
	{
		//currentWeapon = GetComponent<WeaponBase> ();
		if (Guns.Count > 0)
			currentWeapon = Guns[0];
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentWeapon)
		{
			if (currentWeapon.shotOrigin)
			{
				// Should the weapon be up?
				RaycastHit hit = new RaycastHit();
				Physics.Linecast (currentWeapon.shotOrigin.position, transform.position, out hit, layers);

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
						currentWeapon.Execute ();
					}
				}
			}
		}
	}
}
