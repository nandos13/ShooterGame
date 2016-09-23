using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Script that makes a turret find and track the player if in range
 */

/* NOTES TO SELF ON POSSIBLE BEHAVIOUR:
 * Use a raycast first before rotating to player
 * On loss of Line of Sight, revert to original facing position so the turret doesn't
 * end up "camping" a doorway, etc.
 * Maybe use a Coroutine to achieve this
 */

[RequireComponent (typeof(Weapon_Base_Script))]
public class Turret_FollowPlayer : MonoBehaviour {

	public Transform muzzleEnd;				// The location 
	public Transform visionPoint;			// The location which the turret sees out of

	public float VisionAngle;				// Angle in degrees the turret can see the player

	public List<string>SeeThroughTags;		// A list of object tags the turret has vision through
	public bool showTags = false;			// Used in the inspector to track state of Foldout element

	public float TrackingRange;				// The view distance of the turret

	public float MinClampAngle;				// The maximum angle the turret can look DOWN ( 0 = horizontal, 88 = almost directly up )

	public float MaxClampAngle;				// The maximum angle the turret can look UP ( 0 = horizontal, 88 = almost directly up )

	public float ShootDelayOnTargetAcquire;	// Amount of delay in seconds before firing at player

	private GameObject player;				// Reference to the player
	private float distanceToPlayer;			// Track the distance to player
	public float rotateSpeed = 2.0f;		// Speed the turret will rotate at

	private enum BehaviourState 
	{ PreparingFire, Firing, Searching };	
	private BehaviourState state = 
		BehaviourState.Searching;			// State of the turret's behaviour

	private Vector3 randomPoint;			// A point used to randomly search around for the player


	// Use this for initialization
	void Start () 
	{
		player = GameObject.Find ("Player");
		distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check the turret has all the transform data it needs
		if (player && muzzleEnd)
		{
			// Is the player within viewing distance?
			distanceToPlayer = Vector3.Distance (player.transform.position, transform.position);
			//Debug.Log ("Turret distance to player: " + distanceToPlayer.ToString());
			if (distanceToPlayer <= TrackingRange) 
			{
				// Raycast to check if the turret has line of sight to the target
				Vector3 angleMuzzleToPlayer = player.transform.position - muzzleEnd.transform.position;
				RaycastHit hit = new RaycastHit();
				RaycastHit[] hits = Physics.RaycastAll (new Ray (visionPoint.position, angleMuzzleToPlayer), Vector3.Distance(player.transform.position, visionPoint.transform.position) + 0.01f);
				Debug.DrawRay (visionPoint.position, angleMuzzleToPlayer, Color.green);

				// Find first collision that shouldn't be ignored (prevents losing LOS if a bullet, etc gets in the way)
				foreach (RaycastHit h in hits)
				{
					bool hitVisible = true;

					// Ignore the turret itself including all child objects
					if ( !(h.transform.IsChildOf(transform)) )
					{
						// Check if raycast hit an object that should be ignored
						foreach (string s in SeeThroughTags)
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
						Debug.Log(hit.collider.name);
						break;
					}
				}

				if (hit.collider) 
				{
					if (hit.collider.gameObject == player || hit.collider.transform.IsChildOf(player.transform)) 
					{
						// Only allow line of sight if the player is in front of the turret
						float angleDegreesToPlayer = Vector3.Angle (visionPoint.forward, (player.transform.position - visionPoint.transform.position).normalized);
						if (angleDegreesToPlayer < VisionAngle)
						{
							// Turret has line of sight to player, rotate to look at the player

							// Handle turret rotation towards player
							RotateToFacePoint (player.transform);

							// Raycast directly ahead of the turret to see if the player is being aimed at
							Physics.Raycast (new Ray (muzzleEnd.position, muzzleEnd.forward), out hit, TrackingRange);
							Debug.DrawRay (muzzleEnd.position, muzzleEnd.forward * 9.0f, Color.red);

							// Did the ray collide with anything?
							if (hit.collider) 
							{
								// Was it the player?
								if (hit.collider.gameObject == player) 
								{
									// Is the turret ready to fire?
									if (!((state == BehaviourState.PreparingFire) || (state == BehaviourState.Firing))) 
									{
										// Warm up the turret
										state = BehaviourState.PreparingFire;
										Debug.Log ("Warming up turret");
										StartCoroutine (WarmUpGun ());
									}
								}
							}

							TryShoot ();
						}
						else 
						{
							// Turret does not have line of sight to player, stop shooting and go back to searching
							StopShooting();
						}
					} 
					else 
					{
						// Turret does not have line of sight to player, stop shooting and go back to searching
						StopShooting();
					}
				}
				else 
				{
					// Turret does not have line of sight to player, stop shooting and go back to searching
					StopShooting();
				}
			} 
			else 
			{
				// Player is out of range, stop shooting and go back to searching
				StopShooting();
			}
		}
	}

	public IEnumerator WarmUpGun ()
	{
		/* This function provides a delay before allowing the turret to shoot,
		 * to avoid having an overpowered turret that fires at the player 
		 * instantly once they come around a corner.
		 */
		yield return new WaitForSeconds (ShootDelayOnTargetAcquire);
		if (state == BehaviourState.PreparingFire)
			state = BehaviourState.Firing;
		yield return null;
	}

	private void TryShoot ()
	{
		/* This function attempts to fire if the turret is ready
		 */
		if (state == BehaviourState.Firing) 
		{
			// Shoot weapon
			Weapon_Base_Script turretGun = GetComponent<Weapon_Base_Script>();
			turretGun.Execute ();
		}
	}

	private void StopShooting ()
	{
		if (state == BehaviourState.PreparingFire || state == BehaviourState.Firing) 
		{
			Debug.Log ("Stopping Turret (no LoS), now searching randomly");
			state = BehaviourState.Searching;
		}
		SearchRandomly ();
	}

	private void SearchRandomly ()
	{
		/* This function picks a random point for the turret to look
		 * towards while it searches for the player.
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
		Vector3 angleTurretToPoint = (randomPoint - transform.position).normalized;

		if (randomPoint != Vector3.zero) 
		{
			float precision = Vector3.Dot (angleTurretToPoint, transform.forward);

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
			Transform vision = transform.Search("Eyes");
			for (uint i = 0; i < 4; i++) 
			{
				randomPoint = Random.insideUnitSphere;
				randomPoint += transform.position;
				randomPoint.y = transform.position.y;

				// Raycast in the direction of this point to see if there is a wall in close proximity
				angleTurretToPoint = (randomPoint - transform.position);
				RaycastHit hit;
				Physics.Raycast (new Ray (vision.position, angleTurretToPoint), out hit, TrackingRange);
				//Debug.DrawRay (vision.position, angleTurretToPoint, Color.blue, 1.0f);

				if (!hit.collider) 
				{
					// Ray is not looking at a wall (or anything at all)
					break;
				} 
				else 
				{
					// Get the distance between the turret and the object it will be looking at
					float dist = Vector3.Distance (randomPoint, transform.position);

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

		// Store old rotation for later
		Quaternion oldRotation = transform.rotation;

		// Find vector from the turret to the player
		Vector3 angleToPoint = point.position - transform.position;

		// Calculate lateral angle needed to rotate towards specified point (angle for turret to look side to side)
		Vector3 lateralDir = angleToPoint;
		lateralDir.y = 0;
		lateralDir.Normalize ();

		// Ensure the turret will turn the correct way (instead of turning the long way around)
		float lateralAngle = Vector3.Angle (Vector3.forward, lateralDir) * Mathf.Sign (Vector3.Cross (Vector3.forward, lateralDir).y);
		Quaternion lateralRotation = Quaternion.AngleAxis (lateralAngle, Vector3.up);
		transform.rotation = lateralRotation;

		// Calculate medial angle needed to rotate towards specified point (angle for turret to look up and down)
		Vector3 medialDir = angleToPoint;
		medialDir.Normalize ();				
		float medialAngle = Vector3.Angle (transform.forward, medialDir);

		// Clamp medial angle to specified range
		medialAngle = Mathf.Clamp (medialAngle, -MinClampAngle, MaxClampAngle);
		Quaternion otherRotation = Quaternion.AngleAxis (medialAngle, Vector3.Cross (transform.forward, medialDir));

		// While firing, the turret's aiming will be faster so bullets do not lag behind a moving player
		float currentRotateSpeed = rotateSpeed;
		if (state == BehaviourState.Firing)
			currentRotateSpeed *= 2.5f;

		transform.rotation = Quaternion.Slerp (oldRotation, otherRotation * lateralRotation, Time.deltaTime * currentRotateSpeed);
	}
}
