using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Handles gun firing for enemy AI. Rotates specified pieces to face the target,
 * applying clamps in all directions. Fires attached guns, persisting for a 
 * specified time upon loss of Line of Sight.
 */

public class AIGunControl : MonoBehaviour {

	public Transform eyes;															// Vision point
	public bool targetPlayer = true;
	public GameObject target;
	public bool autoGunList = true;													// If true, auto populate the gun list. Else, individual guns can be specified
	public List<WeaponBase> guns = new List<WeaponBase>();							// A list of guns useable by the AI

	public List<RotationPieces> rotationPieces = new List<RotationPieces>();		// A list of all pieces that will rotate to face the target

	public List<string>seeThroughTags = new List<string>();

	[Range (0.0f, 500.0f)]
	public float shotRange = 50.0f;													// Will only shoot if within this range from target
	[Range (0.0f, 3.0f)]
	public float persistTime = 1.5f;												// Time turret will continue to shoot for after losing LOS
	protected bool persistent = false;
	protected IEnumerator persistCR;												// Single instance of persistence coroutine

	void Start ()
	{
		MyStart ();
	}

	protected void MyStart () 
	{
		persistCR = beginGiveUp();

		if (autoGunList)
		{
			// Auto populate gun list with all instances of WeaponBase found on the transform
			WeaponBase[] tempGuns = GetComponents<WeaponBase>();
			guns.Clear();
			foreach (WeaponBase tGun in tempGuns)
				guns.Add(tGun);
		}

		if (targetPlayer)
			target = GameObject.Find("Player");
	}

	void Update () 
	{
		if (!Options.Paused && target && eyes)
		{
			// Is the target within range?
			if (Vector3.Distance (target.transform.position, transform.position) < shotRange)
			{
				// Does the AI have line of sight to the target?
				float distToTarget = Vector3.Distance (eyes.transform.position, target.transform.position);
				RaycastHit hit = new RaycastHit();
				RaycastHit[] hits = Physics.RaycastAll (eyes.position, (target.transform.position - eyes.position), distToTarget);
				if (hits.Length > 0)
				{
					// Ignore specified tags and get first raycastHit that should be visible
					hit = hits.ApplyTagMask (seeThroughTags);

					if (hitIsTarget(hit))
					{
						// AI has line of sight to target. Set persistence so the AI will fire
						persistent = true;
						
						// Stop giving up!
						StopCoroutine(persistCR);
						persistCR = beginGiveUp();
					}
					else
					{
						// No line of sight. Start a coroutine to make the AI "give up" after persistentTime elapses
						StartCoroutine(persistCR);
					}
				}

				// Is the AI persisting with shooting?
				if (persistent)
				{
					// Face player
					facePlayer();

					// Fire all attached guns
					fire();
				}
			}
		}
	}

	protected IEnumerator beginGiveUp ()
	{
		/* After "persistentTime" seconds elapses, the AI will give up
		 * and temporarily stop trying to shoot the player.
		 */
		yield return new WaitForSeconds(persistTime);
		persistent = false;
	}

	protected void facePlayer ()
	{
		foreach (RotationPieces rot in rotationPieces)
		{
			if (rot != null)
			{
				rot.rotateToFace (target.transform);
			}
		}
	}

	protected void fire ()
	{
		/* Fire all attached guns */
		foreach (WeaponBase gun in guns)
		{
			if (gun)
			{
				gun.Execute ();
			}
		}
	}

	protected bool hitIsTarget (RaycastHit hit)
	{
		if (hit.collider)
		{
			if (hit.collider.gameObject == target)
				return true;
		}
		return false;
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine (eyes.position, target.transform.position);
	}
}
