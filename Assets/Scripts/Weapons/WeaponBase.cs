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
	public FIRE_MODE fMode = FIRE_MODE.Auto;								// Will the gun continue to fire if the shoot button is held down?
	public bool bottomlessClip = false;										// Will the gun ever need to be reloaded?
	public uint clipSize = 30;												// Ammo in a single clip
	protected uint currentClip;												// Tracks currently loaded ammo
	public bool unlimitedAmmo = false;										// Will the gun ever run out of ammo?
	public uint startingAmmo = 100;											// Total ammo held on spawn
	protected uint currentAmmoTotal;										// Tracks current total-ammo count
	public float damage = 10;												// Damage done per shot/second
	public List<string> dmgTags = new List<string>();						// A list of tags to either be ignored or exclusively damaged
	public COLLISION_MODE dmgTagsMode = COLLISION_MODE.IgnoreSelected;		// Choose how dmgTags is used
	public float speedRPM = 600;											// Firing speed in Rounds Per Minute. Used in inspector
	public float speed;														// Firing speed in Rounds Per Second. Calculated using speedRPM
	public float spread;													// Accuracy of shots, where 0 = completely accurate
	protected bool canFire = true;											// Tracks whether or not the gun can fire (based on fire rate)
	protected bool canFireSemi = true;										// Tracks whether or not the gun can fire (based on semi-auto/automatic setting)


	/* HEAT MECHANIC VARIABLES */
	public bool useHeatMechanics = false;									// Does this weapon use heat mechanics?
	public float heatRise = 30;												// How much heat will rise each shot (or per second)
	public bool heatOverTime = true;										// if true, heat over time. else heat instant on fire
	public float heatFall = 20;												// How much heat will fall each second
	public float heatFallWait = 1;											// Time in seconds before gun will begin cooling after ceasefire
	public bool instantHeatReset = false;									// If true, heat will instantly hit 0 when cooling
	public float heatReEnable = 80;											// Can begin firing if heat is lower than this
	protected float currentHeat = 0;										// Tracks current heat. Should be clamped 0-100
	protected bool coolDownAvailable = true;								// Tracks if the gun hasn't been fired within heatFallWait time and is able to cool
	protected IEnumerator cooler;


	/* VISUALS VARIABLES */

	public ParticleSystem hitEffect;										// Particle system played where the projectile collides
	public uint hitParticles = 10;											// Number of particles to emit on hit
	protected List<ParticleSystem> hitPool = new List<ParticleSystem>();	// Particles are pooled for use
	public List<MBAction> onFire = new List<MBAction>();					// List of actions to execute on fire


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

		// Initialize cooler coroutine
		cooler = CoolDownWaitTime();
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
			effect.gameObject.hideFlags = HideFlags.HideInHierarchy;
			effect.Pause();

			// Add effect to the pool
			hitPool.Add(effect);

			return effect;
		}
		return default(ParticleSystem);
	}

	public void semiFireEnable ()
	{
		/* Attempts to re enable firing for semi auto weapons */

		if (!canFireSemi)
		{
			if (canFire)
				canFireSemi = true;
		}
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

	protected void RunFireScripts ()
	{
		/* Iterate through the list of onFire scripts and execute */

		foreach (MBAction action in onFire)
		{
			if (action)
				action.Execute();
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

	protected void ApplySpread(ref Vector3 vec)
	{
		/* Applies bullet spread to a Vector3
		 */

		vec.x += Random.Range (-spread / 100.0f, spread / 100.0f);
		vec.y += Random.Range (-spread / 100.0f, spread / 100.0f);
		vec.z += Random.Range (-spread / 100.0f, spread / 100.0f);
	}

	protected void HeatCoolDown ()
	{
		/* Cools the gun over time */
		if (useHeatMechanics && currentHeat > 0 && coolDownAvailable)
		{
			if (instantHeatReset)
				currentHeat = 0;
			else
			{
				currentHeat -= heatFall * Time.deltaTime;
				currentHeat = Mathf.Clamp (currentHeat, 0, 100);
			}
			Debug.Log ("Cooling down: " + currentHeat);
		}
	}

	protected IEnumerator CoolDownWaitTime ()
	{
		/* After the gun has been fired, wait for heatFallTime before enabling cooling */
		yield return new WaitForSeconds(heatFallWait);
		coolDownAvailable = true;
	}

	protected bool checkHeat ()
	{
		/* Check heat mechanics to determine if the gun can fire */
		bool heatFire = false;
		if (useHeatMechanics)
		{
			if (currentHeat < 100)
			{
				if (!canFireSemi)
					heatFire = true;
				else
				{
					if (currentHeat < heatReEnable)
						heatFire = true;
				}
			}
		}
		else
			heatFire = true;

		return heatFire;
	}

	protected void applyHeat ()
	{
		/* Add heat if needed */
		if (useHeatMechanics && currentHeat < 100)
		{
			// Cancel cooldown ability
			coolDownAvailable = false;
			StopCoroutine (cooler);
			cooler = CoolDownWaitTime();

			float generatedHeat = heatRise;
			if (heatOverTime)
			{
				generatedHeat *= Time.deltaTime;
			}

			currentHeat += generatedHeat;
			currentHeat = Mathf.Clamp (currentHeat, 0, 100);

			// Begin cooling down
			StartCoroutine(cooler);
		}
	}
}
