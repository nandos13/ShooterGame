using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Stores basic information about the weapon, including Ammo, Damage, Range, Speed, etc,
 * and handles firing and reloading of these weapons.
 * Several different weapon types allow for different gunplay.
 */

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
	public bool hitscan = false;			// Does this weapon use raycasting or physical bullets?

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
				if (!hitscan) 
				{
					// Spawn bullet objects in pool (for optimized efficiency)
					bulletPoolSize = (uint)(DespawnBulletAfter * Speed) + 1;
					List<Transform> ownerTransforms = transform.GetComponentsDescending<Transform> (true);

					for (uint i = 0; i < bulletPoolSize; i++)
					{
						//TODO: THIS NEEDS TO BE OPTIMIZED ONCE IT IS FINISHED
						GameObject bullet = Instantiate (bulletProjectile) as GameObject;
						bullet.hideFlags = HideFlags.HideInHierarchy;
						bullet.SetActive (false);

						// Get collision handler
						On_Collision collisionHandler = bullet.GetComponent<On_Collision>();
						if ( !(bullet.GetComponent<On_Collision>()) )
						{
							bullet.AddComponent<On_Collision> ();
							collisionHandler = bullet.GetComponent<On_Collision> ();
						}

						// Add despawn conditions
						bullet.AddComponent<Disable_After_Seconds> ();
						bullet.GetComponent<Disable_After_Seconds> ().Delay = DespawnBulletAfter;

						bullet.AddComponent<Disable> ();
						collisionHandler.Actions.Add(bullet.GetComponent<Disable>());

						// Add damage to the bullet
						bullet.AddComponent<ApplyDamage> ();
						bullet.GetComponent<ApplyDamage> ().Damage = Damage;
						collisionHandler.Actions.Add(bullet.GetComponent<ApplyDamage>());

						// TEMPORARY: EXPLOSIONS
						bullet.AddComponent<Explosion> ();
						bullet.GetComponent<Explosion> ().CollisionTags.Add(transform.tag);
						collisionHandler.Actions.Add(bullet.GetComponent<Explosion>());

						// Set collision-ignore-tags
						collisionHandler.CollisionTags.Add(transform.tag);
						collisionHandler.CollisionTags.Add("Bullet");

						// Add bullet to the pool
						bulletPool.Add(bullet);
					}
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

	private Vector3 VectorToCrosshair ()
	{
		/* Calculates and returns the vector between the shot origin and
		 * the surface under the crosshair (in the center of the screen).
		 */

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
			float distance = Vector3.Distance (shotOrigin.transform.position, hit.point);
			Vector3 result = (hit.point - shotOrigin.position).normalized * distance;
			return result;
		}
		else
		{
			// Point 1000 units away from the center of the screen
			Vector3 result = ray.direction.normalized * 1000.0f;
			return result;
		}
	}

	private void ApplySpread(ref Vector3 vec)
	{
		/* Applies bullet spread to a Vector3
		 */

		vec.x += Random.Range (-Spread / 40.0f, Spread / 40.0f);
		vec.y += Random.Range (-Spread / 40.0f, Spread / 40.0f);
		vec.z += Random.Range (-Spread / 40.0f, Spread / 40.0f);
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

					// Does this bullet weapon use instant raycast method or physical bullets?
					if (hitscan)
						ShootType_Bullet_Ray ();
					else
						ShootType_Bullet_ObjectProjectile ();
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
		Vector3 projectAngle;
		if (transform.tag == "Player")
			projectAngle = VectorToCrosshair();
		else
			projectAngle = shotOrigin.forward;
		
		projectAngle.Normalize ();

		// Apply random bullet spread
		ApplySpread(ref projectAngle);

		// Apply bullet projectile force
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
			muzzleFlash.Emit(7);
	}

	private void ShootType_Bullet_Ray ()
	{
		/* NOTE:
		 * Currently I'm not sure if the bullet weapons should shoot a physical bullet
		 * or simply raycast. Once this decision has been made, this function's contents
		 * should be moved back to the main ShootType_Bullet () function.
		 */

		// Get the angle of projectile
		Vector3 projectAngle;
		if (transform.tag == "Player")
			projectAngle = VectorToCrosshair();
		else
			projectAngle = shotOrigin.forward;
		
		projectAngle.Normalize ();

		// Apply random bullet spread
		ApplySpread(ref projectAngle);

		// Raycast from the muzzle to see what the gun hit
		RaycastHit hit = new RaycastHit();
		Physics.Raycast (new Ray (shotOrigin.position, projectAngle), out hit);
		Debug.DrawRay (shotOrigin.position, projectAngle, Color.cyan, 1.0f);

		/* Next we need to get the health script of the object hit. However, it's possible
		 * the ray hit a child of the object with health (eg hit an arm, but the body has the health script).
		 * To achieve this, we use a custom function which will return the most immediate instance
		 * of a component contained by a transform or any parent in its family tree.
		 */
		if ( Physics.Raycast (new Ray (shotOrigin.position, projectAngle)) )
		{
			Health healthComponent = hit.transform.GetComponentAscendingImmediate<Health>(true);

			// Did the ray hit something that has health?
			if (healthComponent)
			{
				healthComponent.ApplyDamage(Damage);
				//Debug.Log("Applying damage to: " + healthComponent.transform.name);
			}
		}

		// Disable firing to maintain fire rate
		StartCoroutine (DisableShootForFireRate ());

		// Show muzzle flash
		if (muzzleFlash) 
			muzzleFlash.Emit(7);
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
