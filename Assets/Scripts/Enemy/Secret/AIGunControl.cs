using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * 
 */

public class AIGunControl : MonoBehaviour {

	public Transform eyes;															// Vision point
	public GameObject target;
	public bool autoGunList = true;													// If true, auto populate the gun list. Else, individual guns can be specified
	public List<WeaponBase> guns = new List<WeaponBase>();							// A list of guns useable by the AI

	public List<RotationPieces> rotationPieces = new List<RotationPieces>();		// A list of all pieces that will rotate to face the target

	[Range (0.0f, 500.0f)]
	public float shotRange = 50.0f;													// Will only shoot if within this range from target
	[Range (1.0f, 10.0f)]
	public float turnSpeed = 2.0f;													// Speed at which turret will turn to face the target
	[Range (0.0f, 3.0f)]
	public float persistTime = 1.5f;												// Time turret will continue to shoot for after losing LOS
	private bool persistent = false;
	private IEnumerator persistCR;													// Single instance of persistence coroutine

	void Start () 
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
	}

	void Update () 
	{
		if (target && eyes)
		{
			// Is the target within range?
			if (Vector3.Distance (target.transform.position, transform.position) < shotRange)
			{
				// Does the AI have line of sight to the target?
				RaycastHit hit = new RaycastHit();
				if (Physics.Linecast (eyes.position, target.transform.position, out hit))
				{
					if (hit.collider.gameObject)
					{
						// Set persistence so the AI will fire
						persistent = true;

						// Stop giving up!
						StopCoroutine(persistCR);
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

	private IEnumerator beginGiveUp ()
	{
		/* After "persistentTime" seconds elapses, the AI will give up
		 * and temporarily stop trying to shoot the player.
		 */
		yield return new WaitForSeconds(persistTime);
		persistent = false;
	}

	private void facePlayer ()
	{
		foreach (RotationPieces rot in rotationPieces)
		{
			if (rot != null)
			{
				rot.rotateToFace (target.transform);
			}
		}
	}

	private void fire ()
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
}
