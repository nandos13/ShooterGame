using UnityEngine;
using System.Collections;

public class BeamWeapon : WeaponBase {

	/* BEAM-SPECIFIC VARIABLES */

	public AnimationCurve pulseCurve = new AnimationCurve ();
	public LineRenderer lr;
	public Transform laserStart;
	protected bool pulseCool = true;
	protected float pulseCoolTime = 1.3f;
	public float hitscanRange = 80.0f;


	/* MEMBER FUNCTIONS */

	void Start () 
	{
		MyStart();
	}

	void Update () 
	{
		// Handle heat fall
		HeatCoolDown ();

		if (lr)
		{
			Vector3[] positions = new Vector3[2];
			positions[0] = laserStart.position;
			positions[1] = laserStart.position;
			lr.SetPositions(positions);
		}
	}

	protected override void MyStart ()
	{
		// Call WeaponBase Start function to initialize basic variables
		base.MyStart();

		type = WEAPON_TYPE.Beam;
		bottomlessClip = true;
		unlimitedAmmo = true;

		StartCoroutine (pulseLine());
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

					Shoot();

					// Disable firing to maintain fire rate
					//StartCoroutine (DisableShootForFireRate ());
					// Probably not used for laser weapons?

					//FlashMuzzle ();
					RunFireScripts ();
				}
				else
				{/* TODO: GUN NEEDS TO BE RELOADED. DO SOMETHING HERE */}
			}
		}
	}

	protected void Shoot ()
	{
		/* Fires a single pulse of energy */

		PlayShotSound ();

		// Get angle of projectile
		Vector3 projectAngle = shotOrigin.forward;

		projectAngle.Normalize();

		// Apply random bullet spread
		ApplySpread(ref projectAngle);

		// Raycast from the muzzle to see what the gun hit
		RaycastHit hit = new RaycastHit();
		Physics.Raycast (new Ray (shotOrigin.position, projectAngle), out hit);
		Debug.DrawRay (shotOrigin.position, projectAngle, Color.cyan, 1.0f);
		Vector3 lineEndPos = Vector3.zero;

		if ( Physics.Raycast (new Ray (shotOrigin.position, projectAngle)) )
		{
			// Check distance
			if (Vector3.Distance(shotOrigin.position, hit.point) <= hitscanRange)
			{
				
				lineEndPos = hit.point;

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
					{
						if (fMode == FIRE_MODE.SemiAuto)
							healthComponent.ApplyDamage(damage);
						else
							healthComponent.ApplyDamage(damage * Time.deltaTime);
					}
				}
			}
			else
			{
				Vector3 tempPoint = hit.point - transform.position;
				tempPoint.Normalize();
				tempPoint *= hitscanRange;
				lineEndPos = tempPoint + transform.position;
			}
		}
		else
		{
			lineEndPos = (projectAngle * hitscanRange) + shotOrigin.position;;
		}

		// Render line
		renderLine(lineEndPos);
	}

	protected void renderLine (Vector3 endPoint)
	{
		if (lr && laserStart)
		{
			lr.SetVertexCount(2);
			lr.SetWidth(0.02f, 0.02f);
			Vector3[] positions = new Vector3[2];
			positions[0] = laserStart.position;
			positions[1] = endPoint;
			lr.SetPositions(positions);
		}
	}

	protected IEnumerator pulseLine ()
	{
		if (pulseCurve != null && lr)
		{
			float timer = 0;
			float initialSize = 0.03f;
			float newSize = 0.03f;
			while (true)
			{
				timer += Time.deltaTime;
				if (timer > pulseCurve.keys[pulseCurve.length - 1].time)
					timer = 0;
				newSize = initialSize * (pulseCurve.Evaluate (timer));
				lr.SetWidth(newSize, newSize);
				yield return null;
			}
		}
	}
}
