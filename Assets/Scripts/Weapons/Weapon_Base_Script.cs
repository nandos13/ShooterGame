using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Stores basic information about the weapon, including Ammo, Damage, Range, Speed, etc,
 * and handles firing and reloading of these weapons.
 * Several different weapon types allow for different gunplay.
 */

public enum WEAPON_TYPE { Bullet, Launcher, Pulse, Beam }			// Different weapon types for diverse gunplay

//TODO: FINISH ALL WEAPON TYPES AND IMPLEMENT RELOADING
//TODO: DISPLAY TRACER FOR RAYCAST SHOTS??? MAYBE??

public class Weapon_Base_Script : MBAction {

	/* GENERAL VARIABLES */
	public WEAPON_TYPE type = WEAPON_TYPE.Bullet;	// Type of weapon
	public Transform shotOrigin;			// Transform where the bullet should come out
	public ParticleSystem muzzleFlash;		// Particle system played at the end of the barrel when a shot is fired

	public bool BottomlessClip = false;		// Will the weapon ever need to reload?
	public uint ClipSize = 30;				// Ammo in a single clip
	private uint currentClip;				// Tracks current ammo in loaded clip
	public bool UnlimitedAmmo = false;		// Can the weapon ever run out of ammo completely?
	public uint StartingAmmo = 100;			// Total ammo count on spawn
	private uint currentAmmoTotal;			// Tracks current ammo count
	public float Damage = 10;				// For Beam weapons, this will be damage per second
	public float SpeedRPM = 600;			// Firing speed in Rounds Per Minute, should not affect Beam weapons
	public float Speed;						// Firing speed in Rounds Per Second, should not affect Beam weapons
	public float Spread;					// Adds randomness to the precision of the weapon

	private bool canFire = true;			// Tracks whether or not the gun can fire


	/* BEAM WEAPON SPECIFIC VARIABLES */
	public float HeatPerSecond = 10.0f;		// The amount of heat/second generated while firing
	public float HeatFalloff = 10.0f;		// The amount of heat/second lost while not firing
	public float TimeBeforeCooldown = 1.0f;	// The amount in seconds after firing the gun will wait before cooling down


	/* BULLET WEAPON SPECIFIC VARIABLES */
	public int bulletForce = 100;			// Initial force of a fired bullet
	public GameObject bulletProjectile;		// Prefab: projectile for Bullet weapons

	public float DespawnBulletAfter = 2.0f;	// Time in seconds a bullet will exist for after firing
	private uint bulletPoolSize = 20;		// Size of the bullet pool
	private List<GameObject> bulletPool = new List<GameObject>();	// Pool of projectiles for BULLET gun to shoot


	/* LAUNCHER WEAPON SPECIFIC VARIABLES */
	// TODO: randomness factor for rocket trajectory
	// TODO: muzzle velocity
	// TODO: explosive power

	// TODO: rocket pool variables


	/* PULSE WEAPON SPECIFIC VARIABLES */
	// TODO: muzzle velocity
	// TODO: custom gravity


	// Use this for initialization
	void Start ()
	{
		// Initialize current ammo
		currentAmmoTotal = StartingAmmo;
		if (StartingAmmo >= ClipSize)
			currentClip = ClipSize;
		else
			currentClip = StartingAmmo;

		// Initialize speed (Rounds per second)
		Speed = SpeedRPM / 60;

		// Initialize type-specific variables
		switch (type) 
		{
		case WEAPON_TYPE.Bullet:
			{
				// Spawn bullet objects in pool (for optimized efficiency)
				bulletPoolSize = (uint)(DespawnBulletAfter * Speed) + 1;

				for (uint i = 0; i < bulletPoolSize; i++)
				{
					GameObject bullet = Instantiate (bulletProjectile) as GameObject;
					bullet.hideFlags = HideFlags.HideInHierarchy;
					bullet.SetActive (false);
					bullet.AddComponent<Bullet_Timer_Deactivate> ();
					bullet.GetComponent<Bullet_Timer_Deactivate> ().DisableAfterSeconds = DespawnBulletAfter;
					bulletPool.Add(bullet);
				}

				break;
			}
		case WEAPON_TYPE.Launcher:
			{
				// TODO: INITIALIZE LAUNCHER

				break;
			}
		case WEAPON_TYPE.Pulse:
			{
				// TODO: INITIALIZE PULSE

				break;
			}
		case WEAPON_TYPE.Beam:
			{
				// TODO: INITIALIZE BEAM

				break;
			}
		}
	}

	// Shoot when Execute() is called
	public override void Execute ()
	{
		// Check which mode of fire should be used based on weapon type
		switch (type) 
		{
		case WEAPON_TYPE.Bullet:
			{
				ShootType_Bullet ();

				break;
			}
		case WEAPON_TYPE.Launcher:
			{
				ShootType_Launcher ();

				break;
			}
		case WEAPON_TYPE.Pulse:
			{
				ShootType_Pulse ();

				break;
			}
		case WEAPON_TYPE.Beam:
			{
				ShootType_Beam ();

				break;
			}
		}
	}

	private IEnumerator DisableShootForFireRate ()
	{
		/* This function provides a delay between bullet shots so that
		 * a gun can not shoot once per frame and overload the scene
		 * with bullets. 
		 * This will allow a constant fire-rate.
		 */
		canFire = false;
		yield return new WaitForSeconds (1 / Speed);
		//Debug.Log ("Ready to fire again");
		canFire = true;
	}

	private void ShootType_Bullet ()
	{
		/* Handles shooting for the Bullet type weapon.
		 */

		// Check firing is not on cooldown
		if (canFire) 
		{
			// Check the gun has an attached muzzle point
			if (shotOrigin) 
			{
				// Is there enough ammo to fire the gun?
				if (currentClip > 0 || BottomlessClip == true) 
				{
					// Consume ammo if needed
					if (!BottomlessClip) 
					{
						currentClip--;
						currentAmmoTotal--;
					}

					// BULLET METHOD TO BE DECIDED, EITHER SHOOT AN OBJECT OR USE A RAYCAST FOR INSTANT HITTING BULLETS
					//ShootType_Bullet_ObjectProjectile ();	// Shoots a physical bullet

					ShootType_Bullet_Ray ();				// Shoots an instant ray
				} 
				else 
				{
					// The gun needs to be reloaded
					// TODO: Play a tick noise to the player to show this
				}
			} 
			else 
			{
				Debug.Log ("Origin Point not set on weapon. Weapon owner: " + gameObject);
			}
		}
	}

	private void ShootType_Bullet_ObjectProjectile ()
	{
		/* NOTE:
		 * Currently I'm not sure if the bullet weapons should shoot a physical bullet
		 * or simply raycast. Once this decision has been made, this function's contents
		 * should be moved back to the main ShootType_Bullet () function.
		 */

		// Get angle of projectile
		// TODO: APPLY SLIGHTLY RANDOM AIM
		Vector3 projectAngle = shotOrigin.forward;
		projectAngle.Normalize ();
		projectAngle *= bulletForce;

		// Find next available bullet in the pool
		GameObject bullet = null;
		for (int i = 0; i < bulletPool.Count; i++) 
		{
			if (bulletPool [i].activeSelf == false) 
			{
				bullet = bulletPool [i];
				break;
			}
		}
		if (bullet == null) 
		{
			// Add new bullet to the pool
			bullet = Instantiate (bulletProjectile) as GameObject;
			bulletPool.Add (bullet);
		}

		// Reset projectile
		Rigidbody rb = bullet.GetComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		bullet.transform.position = shotOrigin.position;
		bullet.transform.rotation = shotOrigin.rotation;
		bullet.SetActive (true);

		// Fire projectile
		rb.AddForce (projectAngle, ForceMode.Impulse);

		// Disable firing to maintain fire rate
		StartCoroutine (DisableShootForFireRate ());

		// Show muzzle flash
		if (muzzleFlash)
			muzzleFlash.Play();
	}

	private void ShootType_Bullet_Ray ()
	{
		/* NOTE:
		 * Currently I'm not sure if the bullet weapons should shoot a physical bullet
		 * or simply raycast. Once this decision has been made, this function's contents
		 * should be moved back to the main ShootType_Bullet () function.
		 */

		// Get the angle of projectile
		// TODO: APPLY SLIGHTLY RANDOM AIM
		Vector3 projectAngle = shotOrigin.forward;
		projectAngle.Normalize ();

		// Raycast from the muzzle to see what the gun hit
		RaycastHit hit;
		Physics.Raycast (new Ray (shotOrigin.position, projectAngle), out hit);

		/* Next we need to get the health script of the object hit. However, it's possible
		 * the ray hit a child of the object with health (eg hit an arm, but the body has the health script).
		 * To achieve this, we use a custom function which will return all instances of a component
		 * contained by a transform or any parent in its family tree.
		 */
		if (hit.collider)
		{
			List<Health> healthComponents = hit.transform.GetComponentsAscending<Health>();
			
			// Did the ray hit something that has health?
			Debug.Log(healthComponents.Count);
			if (healthComponents.Count > 0)
			{
				foreach (Health h in healthComponents)
				{
					h.ApplyDamage(Damage);
					//Debug.Log("Applying damage to: " + h.transform.name);
				}
			}
		}

		// Disable firing to maintain fire rate
		StartCoroutine (DisableShootForFireRate ());

		// Show muzzle flash
		if (muzzleFlash) 
		{
			muzzleFlash.Play();
		}
	}

	private void ShootType_Launcher ()
	{
		// Fires slower rocket
		// TODO: Shouldnt use standard gravity, but use manual low-gravity and slow movement
		// to give the rocket-propelled feel

		// Check firing is not on cooldown
		if (canFire) 
		{
			// Check the gun has an attached muzzle point
			if (shotOrigin) 
			{
				// Is there enough ammo to fire the gun?
				if (currentClip > 0 || BottomlessClip == true) 
				{
					// Consume ammo if needed
					if (!BottomlessClip) 
					{
						currentClip--;
						currentAmmoTotal--;
					}

					// TODO
				}
			}
		}
	}

	private void ShootType_Pulse ()
	{
		// Fires energy pulse which travels like a bullet with no gravity.

		// Check firing is not on cooldown
		if (canFire) 
		{
			// Check the gun has an attached muzzle point
			if (shotOrigin) 
			{
				// Is there enough ammo to fire the gun?
				if (currentClip > 0 || BottomlessClip == true) 
				{
					// Consume ammo if needed
					if (!BottomlessClip) 
					{
						currentClip--;
						currentAmmoTotal--;
					}

					// TODO
				}
			}
		}
	}

	private void ShootType_Beam ()
	{
		// Fires a continuous beam using a raycast rather than a transform.

		// Beam weapons do not use ammo
	}
}
