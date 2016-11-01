using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Launcher class weapon. Shoots a slower travelling missile.
 */

public class LauncherWeapon : WeaponBase {

	/* BULLET-SPECIFIC VARIABLES */

	public int muzzleVelocity = 100;									// Initial force of a fired missile
	public GameObject missileProjectile;								// A prefab-object fired from the launcher
	public float despawnAfter = 5.0f;									// Time in seconds a missile will exist for after firing
	public float projectileGravity = 1.0f;								// Custom gravity to be added to the projectile
	public float projectileDrag = 0.0f;									// Custom drag for missile rigidbody
	public float projectileAngularDrag = 0.0f;							// Custom angular drag for missile rigidbody
	protected List<GameObject> missilePool = new List<GameObject>();	// Projectiles are pooled for use
	public float ExplosiveForce = 5.0f;									// Explosive force on missile collision
	public float ExplosionRange = 5.0f;									// Range of explosion


	/* MEMBER FUNCTIONS */

	void Start ()
	{
		MyStart ();
	}

	protected override void MyStart () 
	{
		// Call WeaponBase Start function to initialize basic variables
		base.MyStart();

		type = WEAPON_TYPE.Launcher;

		// Initialize the missile pool
		uint poolSize = (uint)(despawnAfter * speed) + 1;

		for (uint i = 0; i < poolSize; i++)
		{
			AddMissileToPool ();
		}
	}

	private GameObject AddMissileToPool ()
	{
		/* Instantiate a pool of missiles. This prevents new objects constantly
		 * being created and allocating memory. This is used to increase
		 * game performance.
		 */

		if (missileProjectile)
		{
			// Instantiate a new bullet
			GameObject missile = Instantiate (missileProjectile) as GameObject;
			missile.hideFlags = HideFlags.HideInHierarchy;
			missile.SetActive (false);

			// Add despawn conditions
			DisableAfterSeconds disTime = missile.GetComponent<DisableAfterSeconds>();
			if (disTime)
				disTime.Delay = despawnAfter;

			// Apply settings to missile explosion
			Explosion expl = missile.GetComponent<Explosion> ();
			if (expl)
			{
				expl.Damage = damage;
				expl.mode = dmgTagsMode;
				if (expl.mode == COLLISION_MODE.IgnoreSelected)
					expl.collisionTags.Add(transform.tag);
				foreach (string str in dmgTags)
					expl.collisionTags.Add(str);
				
				expl.ExplosiveForce = ExplosiveForce;
				expl.Radius = ExplosionRange;
			}

			// Apply hit effect settings
			EmitParticle eP = missile.GetComponent<EmitParticle>();
			if (eP)
			{
				eP.amount = hitParticles;
				eP.particles = AddHitEffectToPool ();
			}

			// Gravity & rigidbody settings
			Rigidbody rb = missile.GetComponent<Rigidbody> ();
			rb.drag = projectileDrag;
			rb.angularDrag = projectileAngularDrag;
			rb.useGravity = false;
			if (projectileGravity > 0)
			{
				missile.AddComponent<ApplyGravity> ();
				missile.GetComponent<ApplyGravity> ().gravity = projectileGravity;
			}

			// Set collision-ignore-tags
			On_Collision oC = missile.GetComponent<On_Collision>();
			if (oC)
				oC.collisionTags.Add(transform.tag);

			// Add bullet to the pool
			missilePool.Add(missile);

			return missile;
		}
		return default(GameObject);
	}

	public override void Execute ()
	{
		/* Handles weapon firing */

		if (!Options.Paused && enabled && transform.gameObject.activeSelf)
		{
			bool semiFire = true;
			if (fMode == FIRE_MODE.SemiAuto)
				semiFire = canFireSemi;

			// Check firing is not on cooldown, and the gun has an attached muzzle point
			if (canFire && semiFire && shotOrigin && checkHeat())
			{
				// Is there enough ammo to fire the gun?
				if (currentClip > 0 || bottomlessClip == true)
				{
					// Consume ammo if needed
					if (!bottomlessClip)
					{
						currentClip--;
						currentAmmoTotal--;
					}

					// Apply heat
					applyHeat();
					if (useHeatMechanics)
						Debug.Log("Current Heat: " + currentHeat);

					// Disable semi fire auto
					canFireSemi = false;

					ShootMissile ();

					// Disable firing to maintain fire rate
					StartCoroutine (DisableShootForFireRate ());

					//FlashMuzzle ();
					RunFireScripts ();

					PlayShotSound ();
				}
				else
				{/* TODO: GUN NEEDS TO BE RELOADED. DO SOMETHING HERE */}
			}
		}
	}

	private void ShootMissile ()
	{
		/* Fires the gun using a physical projectile */

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
		projectAngle *= muzzleVelocity;

		// Find next available missile in the pool
		GameObject missile = null;
		for (int i = 0; i < missilePool.Count; i++) 
		{
			if (missilePool [i].activeSelf == false) 
			{
				missile = missilePool [i];
				break;
			}
		}
		if (missile == null) 
		{
			// Add new missile to the pool
			missile = AddMissileToPool ();
		}

		// Reset projectile
		if (missile)
		{
			Rigidbody rb = missile.GetComponent<Rigidbody> ();
			if (rb)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				missile.transform.position = shotOrigin.position;
				missile.transform.rotation = shotOrigin.rotation;
				missile.SetActive (true);

				// Fire projectile
				rb.AddForce (projectAngle, ForceMode.Impulse);
			}
			else
				Debug.Log("Attempted to fire a missile projectile without a rigidbody!");
		}
	}
}
