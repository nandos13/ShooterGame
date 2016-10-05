using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Base class for all weapons. Stores basic information including Ammo,
 * Damage Values, Range, Speed, etc.
 * All usable weapons should inherit from this class.
 */

public abstract class WeaponBase : MBAction
{

	/* GENERAL VARIABLES */

	protected WEAPON_TYPE type;												// Type of weapon.
	public Transform shotOrigin;											// Transform at the end of the muzzle where projectiles start
	public bool bottomlessClip = false;										// Will the gun ever need to be reloaded?
	public uint clipSize = 30;												// Ammo in a single clip
	protected uint currentClip;												// Tracks currently loaded ammo
	public bool unlimitedAmmo = false;										// Will the gun ever run out of ammo?
	public uint startingAmmo = 100;											// Total ammo held on spawn
	protected uint currentAmmoTotal;										// Tracks current total-ammo count
	public float damage = 10;												// Damage done per shot/second
	public float speedRPM = 600;											// Firing speed in Rounds Per Minute. Used in inspector
	public float speed;														// Firing speed in Rounds Per Second. Calculated using speedRPM
	public float spread;													// Accuracy of shots, where 0 = completely accurate
	protected bool canFire = true;											// Tracks whether or not the gun can fire


	/* VISUALS VARIABLES */

	public ParticleSystem muzzleFlash;										// Particle system played when a shot is fired
	public uint muzzleParticles = 10;										// Number of particles to emit at muzzle
	public ParticleSystem hitEffect;										// Particle system played where the projectile collides
	public uint hitParticles = 10;											// Number of particles to emit on hit
	protected List<ParticleSystem> hitPool = new List<ParticleSystem>();	// Particles are pooled for use


	/* AUDIO VARIABLES */

	public AudioSource audioSrc;											// Audio Source
	public List<AudioClip> shotSound = new List<AudioClip>();				// Shot sounds. One will be picked randomly


	/* MEMBER FUNCTIONS */

	void Start ()
	{
		MyStart ();
	}

	protected virtual void MyStart () 
	{
		/* Initializes basic variables which rely on data from
		 * other public variables.
		 */

		// Initialize current ammo
		currentAmmoTotal = startingAmmo;
		if (startingAmmo >= clipSize)
			currentClip = clipSize;
		else
			currentClip = startingAmmo;

		// Initialize the hit effect pool
		AddHitEffectToPool ();

		// Initialize audio
		if (audioSrc)
		{
			audioSrc.playOnAwake = false;
			if (shotSound.Count > 0)
				audioSrc.clip = shotSound[0];
		}
	}

	protected ParticleSystem AddHitEffectToPool ()
	{
		/* Instantiate a pool of hit effects. This prevents new objects constantly
		 * being created and allocating memory. This is used to increase
		 * game performance.
		 */

		if (hitEffect)
		{
			// Instantiate a new particle system
			ParticleSystem effect = Instantiate (hitEffect) as ParticleSystem;
			//TODO: FIX HIDE IN HEIRARCHY
			effect.hideFlags = HideFlags.HideInHierarchy;
			effect.Pause();

			// Add effect to the pool
			hitPool.Add(effect);

			return effect;
		}
		return default(ParticleSystem);
	}

	protected void PlayShotSound ()
	{
		/* Pick a sound from the list of shot sounds and play it */

		if (audioSrc)
		{
			if (shotSound.Count > 0)
			{
				// Select a random sound
				int soundIndex = Random.Range(0, shotSound.Count);
			
				// Set volume
				float soundVol = Random.Range(0.8f, 1.0f);
				audioSrc.volume = soundVol;
				audioSrc.pitch = Random.Range(0.75f, 1.25f);

				// Play sound
				audioSrc.PlayOneShot(shotSound[soundIndex]);
			}
		}
	}

	protected void FlashMuzzle ()
	{
		if (muzzleFlash)
		{
			muzzleFlash.Emit((int)muzzleParticles);
		}
	}

	protected IEnumerator DisableShootForFireRate ()
	{
		/* This function provides a delay between bullet shots so that
		 * a gun can not shoot once per frame and overload the scene
		 * with bullets. 
		 * This will allow a constant fire-rate.
		 */

		canFire = false;
		yield return new WaitForSeconds (1 / speed);
		canFire = true;
	}

	protected Vector3 VectorToCrosshair ()
	{
		/* Calculates and returns the vector between the shot origin and
		 * the surface under the crosshair (in the center of the screen).
		 * 
		 * ----PLEASE NOTE----
		 * Should only be called to calculate the aim direction of the
		 * player's gun. Do not use for any enemy weapons.
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

	protected void ApplySpread(ref Vector3 vec)
	{
		/* Applies bullet spread to a Vector3
		 */

		vec.x += Random.Range (-spread / 100.0f, spread / 100.0f);
		vec.y += Random.Range (-spread / 100.0f, spread / 100.0f);
		vec.z += Random.Range (-spread / 100.0f, spread / 100.0f);
	}
}
