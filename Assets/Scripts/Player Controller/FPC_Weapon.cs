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
					// Primary fire will handle all auto weapons, secondary fire will handle all semiauto weapons

					// Has the player tried to primary-fire?
					if (Input.GetAxisRaw ("Fire1") > 0) 
					{
						foreach (WeaponBase wb in Guns)
						{
							if (wb)
							{
								if (wb.fMode == FIRE_MODE.Auto)
									wb.Execute ();
							}
						}
					}

					// Has the player tried to secondary-fire?
					if (Input.GetAxisRaw ("Fire2") > 0)
					{
						foreach (WeaponBase wb in Guns)
						{
							if (wb)
							{
								if (wb.fMode == FIRE_MODE.SemiAuto)
									wb.Execute ();
							}
						}
					}
					else
					{
						// Iterate through weapons and reset the semi-auto firing tracker
						foreach (WeaponBase wb in Guns)
						{
							if (wb)
								wb.canFireSemi = true;
						}
					}
				}
			}
		}
	}
}
