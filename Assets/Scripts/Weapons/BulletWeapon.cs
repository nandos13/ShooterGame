using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Bullet class weapon. Has 2 methods for shooting bullets: Projectile and hitscan.
 * Intended for most standard human guns.
 */

public class BulletWeapon : WeaponBase 
{

	/* BULLET-SPECIFIC VARIABLES */

	public bool hitscan = false;										// Will the gun fire physical bullets or use instant raycasts?
	public int bulletForce = 100;										// Initial force of a fired bullet
	public GameObject bulletProjectile;									// A prefab-object fired from the gun
	public float despawnAfter = 2.0f;									// Time in seconds a bullet will exist for after firing
	protected List<GameObject> bulletPool = new List<GameObject>();		// Projectiles are pooled for use


	/* MEMBER FUNCTIONS */

	void Start ()
	{
		MyStart ();
	}

	protected override void MyStart () 
	{
		// Call WeaponBase Start function to initialize basic variables
		base.MyStart();

		type = WEAPON_TYPE.Bullet;

		// Initialize the bullet pool if the gun is not using hitscan
		if (!hitscan) 
		{
			uint bulletPoolSize = (uint)(despawnAfter * speed) + 1;
			
			for (uint i = 0; i < bulletPoolSize; i++)
			{
				AddBulletToPool ();
			}
		}
	}

	private GameObject AddBulletToPool ()
	{
		/* Instantiate a pool of bullets. This prevents new objects constantly
		 * being created and allocating memory. This is used to increase
		 * game performance.
		 */

		if (bulletProjectile)
		{
			//TODO: THIS NEEDS TO BE OPTIMIZED ONCE IT IS FINISHED
			// Instantiate a new bullet
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
			bullet.GetComponent<Disable_After_Seconds> ().Delay = despawnAfter;
			
			bullet.AddComponent<Disable> ();
			collisionHandler.Actions.Add(bullet.GetComponent<Disable>());
			
			// Add damage to the bullet
			bullet.AddComponent<ApplyDamage> ();
			bullet.GetComponent<ApplyDamage> ().Damage = damage;
			collisionHandler.Actions.Add(bullet.GetComponent<ApplyDamage>());

			// Add hit effect
			bullet.AddComponent<EmitParticle> ();
			bullet.GetComponent<EmitParticle> ().amount = hitParticles;
			bullet.GetComponent<EmitParticle> ().particles = AddHitEffectToPool ();
			collisionHandler.Actions.Add(bullet.GetComponent<EmitParticle>());
			
			// Set collision-ignore-tags
			collisionHandler.CollisionTags.Add(transform.tag);
			collisionHandler.CollisionTags.Add("Bullet");
			
			// Add bullet to the pool
			bulletPool.Add(bullet);
			
			return bullet;
		}
		return default(GameObject);
	}

	public override void Execute ()
	{
		/* Handles weapon firing */

		// Check firing is not on cooldown, and the gun has an attached muzzle point
		if (canFire && shotOrigin)
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

				// Does this bullet weapon use instant raycast method or physical bullets?
				if (hitscan)
					ShootHitScan ();
				else
					ShootProjectile ();

				// Disable firing to maintain fire rate
				StartCoroutine (DisableShootForFireRate ());

				FlashMuzzle ();

				PlayShotSound ();
			}
			else
			{/* TODO: GUN NEEDS TO BE RELOADED. DO SOMETHING HERE */}
		}
	}

	protected void ShootProjectile ()
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
			bullet = AddBulletToPool ();
		}

		// Reset projectile
		if (bullet)
		{
			Rigidbody rb = bullet.GetComponent<Rigidbody> ();
			if (rb)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				bullet.transform.position = shotOrigin.position;
				bullet.transform.rotation = shotOrigin.rotation;
				bullet.SetActive (true);
				
				// Fire projectile
				rb.AddForce (projectAngle, ForceMode.Impulse);
			}
			else
				Debug.Log("Attempted to fire a bullet projectile without a rigidbody!");
		}
	}

	protected void ShootHitScan ()
	{
		/* Fires the gun using an instant raycast */

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

		if ( Physics.Raycast (new Ray (shotOrigin.position, projectAngle)) )
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
				healthComponent.ApplyDamage(damage);

			// Apply a force if the object has a rigid body
			Rigidbody rb = hit.transform.GetComponent<Rigidbody> ();
			if (rb)
			{
				Vector3 force = projectAngle.normalized * bulletForce * 20;
				rb.AddForceAtPosition (force, hit.point);
			}
		}
	}
}
