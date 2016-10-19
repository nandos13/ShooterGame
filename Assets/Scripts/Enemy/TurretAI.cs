using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * AI Script for a simple turret enemy. Tracks the player when the player
 * is visible and in range, and randomly searches around when the player is
 * not visible. Also handles shooting.
 */

public class TurretAI : MonoBehaviour {

	public Transform rotationPiece;								// The part of the turret that will rotate to look towards the target
	public float rotateSpeed = 2.0f;							// Speed of rotation
	public float visionAngle;									// Angle in degrees the turret can see the target
	public List<string>seeThroughTags = new List<string>();		// List of object tags the turret has vision through
	public bool autoPopulateGunList = true;						// Will the script use GetComponent to automatically fill the guns list?
	public List<WeaponBase> guns = new List<WeaponBase>();		// List of guns attached to the turret
	public float trackingRange;									// View distance of the turret
	public float clampAngleDown;								// Max angle the turret can look down
	public float clampAngleUp;									// Max angle the turret can look up
	public float shootDelay;									// Delay in seconds before starting to fire

	private GameObject target;									// The turret's target (currently will only ever be the player)
	private TURRET_BEHAVIOUR_STATE state = 
		TURRET_BEHAVIOUR_STATE.Searching;						// Tracks current behaviour being acted out
	private Vector3 randomPoint;								// A point used to randomly search around for the target

	void Start () 
	{
		target = GameObject.Find ("Player");

		// Auto populate gun list
		if (autoPopulateGunList)
		{
			WeaponBase[] tempGuns = GetComponents<WeaponBase>();
			guns = new List<WeaponBase> ();
			foreach (WeaponBase gun in tempGuns)
			{
				guns.Add(gun);
			}
		}
	}

	void Update () 
	{
		// Does the turret have a target?
		if (target)
		{
			// Is the target within viewing distance?
			float distToTarget = Vector3.Distance (target.transform.position, rotationPiece.transform.position);
			if (distToTarget <= trackingRange) 
			{
				// Only allow line of sight if the player is in front of the turret
				float angleDegreesToPlayer = Vector3.Angle (rotationPiece.forward, (target.transform.position - rotationPiece.transform.position).normalized);
				if (angleDegreesToPlayer < visionAngle)
				{
					// Raycast to check if the turret has line of sight to the target
					Vector3 angleToTarget = target.transform.position - rotationPiece.transform.position;
					RaycastHit hit = new RaycastHit();
					RaycastHit[] hits = Physics.RaycastAll (new Ray (rotationPiece.position, angleToTarget), Vector3.Distance(target.transform.position, rotationPiece.transform.position) + 0.01f);
					Debug.DrawRay (rotationPiece.position, angleToTarget, Color.green);
					
					// Find first collision that shouldn't be ignored (prevents losing LOS if a bullet, etc gets in the way)
					foreach (RaycastHit h in hits)
					{
						bool hitVisible = true;
					
						// Ignore the turret itself including all child objects
						if ( !(h.transform.IsChildOf(transform)) )
						{
							// Check if raycast hit an object that should be ignored
							foreach (string s in seeThroughTags)
							{
								if (h.collider.tag == s)
								{
									hitVisible = false;
									break;
								}
							}
						}
						else
							hitVisible = false;
					
						if (hitVisible)
						{
							hit = h;
							break;
						}
					}
					
					if (hit.collider) 
					{
						if (hit.collider.gameObject == target || hit.collider.transform.IsChildOf(target.transform)) 
						{
							// Turret has line of sight to player, rotate to look at the player
							RotateToFacePoint (target.transform);

							// Is the target close to directly in front of the turret?
							if (angleDegreesToPlayer <= 5.0f)
							{
								if (!((state == TURRET_BEHAVIOUR_STATE.PreparingFire) || (state == TURRET_BEHAVIOUR_STATE.Firing))) 
								{
									// Warm up the turret
									state = TURRET_BEHAVIOUR_STATE.PreparingFire;
									Debug.Log ("Warming up turret");
									StartCoroutine (WarmUpGun ());
								}
							}

							TryShoot ();
						} 
						else 
							StopShooting();
					}
					else 
						StopShooting();
				}
				else 
					StopShooting();
			} 
			else 
				StopShooting();
		}
	}

	public IEnumerator WarmUpGun ()
	{
		/* This function provides a delay before allowing the turret to shoot,
		 * to avoid having an overpowered turret that fires at the player 
		 * instantly once they come around a corner.
		 */
		yield return new WaitForSeconds (shootDelay);
		if (state == TURRET_BEHAVIOUR_STATE.PreparingFire)
			state = TURRET_BEHAVIOUR_STATE.Firing;
		yield return null;
	}

	private void TryShoot ()
	{
		/* This function attempts to fire if the turret is ready
		 */
		if (state == TURRET_BEHAVIOUR_STATE.Firing) 
		{
			// Shoot weapon
			foreach (WeaponBase gun in guns)
				gun.Execute ();
		}
	}

	private void StopShooting ()
	{
		if (state == TURRET_BEHAVIOUR_STATE.PreparingFire || state == TURRET_BEHAVIOUR_STATE.Firing) 
		{
			Debug.Log (transform.name + ": Stopping Turret (line of sight lost), now searching randomly");
			state = TURRET_BEHAVIOUR_STATE.Searching;
		}
		SearchRandomly ();
	}

	private void SearchRandomly ()
	{
		/* This function picks a random point for the turret to look
		 * towards while it searches for the target.
		 * 
		 * NOTE: There are some issues with the way this function behaves.
		 * The raycast to check for obscuring objects originates from the turret's current
		 * vision transform rather than the position it would be in after movement.
		 * Also, the raycast does not currently check for the turret itself, 
		 * so it may return inaccurate collisions that would not be a problem.
		 * Despite these issues, the behaviour still works and the turret searches fairly randomly.
		 */

		// Is the turret looking at it's target already?
		bool needsNewTarget = false;
		Vector3 angleTurretToPoint = (randomPoint - rotationPiece.transform.position).normalized;

		if (randomPoint != Vector3.zero) 
		{
			float precision = Vector3.Dot (angleTurretToPoint, rotationPiece.transform.forward);

			if (precision == 1) 
			{
				// Turret is looking at it's target point and must choose a new one to look at
				needsNewTarget = true;
			}
		} 
		else 
		{
			// Turret does not have a target point and needs to choose a new point to look at
			needsNewTarget = true;
		}

		// Choose a new target point to look at
		if (needsNewTarget) 
		{
			// Try 4 times to find a good direction which will not cause the
			// turret to stare at a wall
			for (uint i = 0; i < 4; i++) 
			{
				randomPoint = Random.insideUnitSphere;
				randomPoint += rotationPiece.transform.position;
				randomPoint.y = rotationPiece.transform.position.y;

				// Linecast from turret to random point to check if any walls are obstructing the turret's view in that direction
				//Physics.Linecast (rotationPiece.transform.position, randomPoint, out hit);

				// Raycast in the direction of this point to see if there is a wall in close proximity
				angleTurretToPoint = (randomPoint - rotationPiece.transform.position);
				RaycastHit hit = new RaycastHit ();
				RaycastHit[] hits = Physics.RaycastAll (new Ray (rotationPiece.position, angleTurretToPoint), trackingRange);
				//Debug.DrawRay (vision.position, angleTurretToPoint, Color.blue, 1.0f);

				// Find first collision that isnt the turret
				foreach (RaycastHit h in hits)
				{
					bool hitVisible = true;

					// Ignore the turret itself including all child objects
					if ( !(h.transform.IsChildOf(transform)) )
					{
						if (h.transform.gameObject == target || h.transform.IsChildOf(target.transform))
						{
							// Found the target
							hitVisible = false;
							break;
						}
					}
					else
						hitVisible = false;

					if (hitVisible)
					{
						hit = h;
						break;
					}
				}

				if (!hit.collider) 
				{
					// Ray is either looking at the player or nothing at all. Not a wall.
					break;
				} 
				else 
				{
					// Get the distance between the turret and the object it will be looking at
					float dist = Vector3.Distance (hit.point, rotationPiece.transform.position);
				
					// Is the object too close?
					if (! (dist < 5.5f) ) 
					{
						// This direction is not looking directly at a wall, break out of loop
						break;
					}
				}
			}
		}

		// Look at the target
		GameObject temp = new GameObject();
		temp.transform.position = randomPoint;
		RotateToFacePoint (temp.transform);
		Destroy (temp);
	}

	private void RotateToFacePoint (Transform point)
	{
		/* This function handles the turret rotation.
		 * The turret will smoothly look towards any point specified
		 * in the parameter 'point', clamping any vertical rotation
		 * to the minimum and maximum angle variables.
		 */

		if (rotationPiece)
		{
			// Store old rotation for later
			Quaternion oldRotation = rotationPiece.transform.rotation;

			// Find vector from the turret to the player
			Vector3 angleToPoint = point.position - rotationPiece.transform.position;

			// Calculate lateral angle needed to rotate towards specified point (angle for turret to look side to side)
			Vector3 lateralDir = angleToPoint;
			lateralDir.y = 0;
			lateralDir.Normalize ();

			// Ensure the turret will turn the correct way (instead of turning the long way around)
			float lateralAngle = Vector3.Angle (Vector3.forward, lateralDir) * Mathf.Sign (Vector3.Cross (Vector3.forward, lateralDir).y);
			Quaternion lateralRotation = Quaternion.AngleAxis (lateralAngle, Vector3.up);
			rotationPiece.transform.rotation = lateralRotation;

			// Calculate medial angle needed to rotate towards specified point (angle for turret to look up and down)
			Vector3 medialDir = angleToPoint;
			medialDir.Normalize ();				
			float medialAngle = Vector3.Angle (rotationPiece.transform.forward, medialDir);

			// Clamp medial angle to specified range
			medialAngle = Mathf.Clamp (medialAngle, -clampAngleDown, clampAngleUp);
			Quaternion otherRotation = Quaternion.AngleAxis (medialAngle, Vector3.Cross (rotationPiece.transform.forward, medialDir));

			// While firing, the turret's aiming will be faster so bullets do not lag behind a moving player
			float currentRotateSpeed = rotateSpeed;
			if (state == TURRET_BEHAVIOUR_STATE.Firing)
				currentRotateSpeed *= 2.5f;

			rotationPiece.transform.rotation = Quaternion.Slerp (oldRotation, otherRotation * lateralRotation, Time.deltaTime * currentRotateSpeed);
		}
	}
}
