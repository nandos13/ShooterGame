using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Scatter class weapon. Used for shotguns. Uses raycasts similar to the standard bullet 
 * weapon script.
 */

public class ScatterBulletWeapon : WeaponBase 
{

	/* SCATTER-SPECIFIC VARIABLES */

	public float hitscanRange = 80.0f;
	public int bulletForce = 100;
	public uint projectileCount = 10;


	/* MEMBER FUNCTIONS */

	void Start () 
	{
		MyStart ();
	}

	void Update () 
	{
		// Handle heat fall
		HeatCoolDown ();
	}

	protected override void MyStart ()
	{
		// Call WeaponBase Start function to initialize basic variables
		base.MyStart();

		type = WEAPON_TYPE.Scatter;
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
					//if (useHeatMechanics)
					//	Debug.Log("Current Heat: " + currentHeat);

					// Disable semi fire auto
					canFireSemi = false;

					ShootScatter ();

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

	protected void ShootScatter ()
	{
		for (int projCount = 0; projCount < projectileCount; projCount++)
		{
			/* Fires the gun using an instant raycast */

			// Get the angle of projectile
			Vector3 projectAngle = shotOrigin.forward;
			//projectAngle.Normalize();

			// Apply random bullet spread
			ApplySpread(ref projectAngle);

			// Raycast from the muzzle to see what the gun hit
			Ray ray = new Ray (shotOrigin.position, projectAngle);
			RaycastHit hit = new RaycastHit();
			Physics.Raycast (ray, out hit, hitscanRange);
			Debug.DrawRay (shotOrigin.position, projectAngle, Color.cyan, 1.0f);

			if ( hit.collider )
			{
				// Display hit particle effect where the ray collides
				if (hitEffect)
				{
					// Find next available hit effect particle system in the pool
					ParticleSystem effect = null;
					for (int i = 0; i < hitPool.Count; i++) 
					{
						if (hitPool [i].IsAlive() == false) 
						{
							effect = hitPool [i];
							break;
						}
					}
					if (effect == null) 
					{
						// Add new particle system to the pool
						effect = AddHitEffectToPool ();
					}

					effect.transform.position = hit.point;
					effect.Emit((int)hitParticles);
				}

				/* Next we need to get the health script of the object hit. However, it's possible
		 		 * the ray hit a child of the object with health (eg hit an arm, but the body has the health script).
		 		 * To achieve this, we use a custom function which will return the most immediate instance
		 		 * of a component contained by a transform or any parent in its family tree.
		 		 */
				Health healthComponent = hit.transform.GetComponentAscendingImmediate<Health>(true);

				// Did the ray hit something that has health?
				if (healthComponent)
				{
					// Should the hit deal damage or be ignored?
					bool dmgIgnore = false;
					if (dmgTagsMode == COLLISION_MODE.HitSelected)
						dmgIgnore = true;

					foreach (string str in dmgTags)
					{
						if (healthComponent.transform.tag == str)
						{
							if (dmgTagsMode == COLLISION_MODE.IgnoreSelected)
							{
								dmgIgnore = true;
								break;
							}
							else if (dmgTagsMode == COLLISION_MODE.HitSelected)
							{
								dmgIgnore = false;
								break;
							}
						}
					}

					if (!dmgIgnore)
						healthComponent.ApplyDamage(damage);
				}

				// Apply force if the object has a rigid body
				Rigidbody rb = hit.transform.GetComponent<Rigidbody> ();
				if (rb)
				{
					Vector3 force = projectAngle.normalized * bulletForce * 20;
					rb.AddForceAtPosition (force, hit.point);
				}
			}
		}
	}
}
