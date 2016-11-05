using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * This script handles movement for the player, including walking and jumping.
 */

/* TODO: (Coding, Designers ignore this)
 * Rewrite this code.
 * Use the Normal angle of the surface the player is walking on to affect 
 * the vertical force and prevent the bunny-hop issue.
 * Implement maximum walking slope and more directly controlled walking conditions
 * rather than just adding force regardless.
 */

//wiki.unity3d.com/index.php?title=RigidbodyFPSWalker

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
public class FPC_WalkOLD : MonoBehaviour {

	[Range (1.0f, 50.0f)]
	public float speed = 10.0f;					// Walk speed
	public float gravity = 9.8f;				// Manual gravity factor
	public float maxVelocityChange = 8.0f;		// Lower values mean more acceleration, feels like ice skates
	public bool canJump = true;					// Whether or not the player can jump
	public float jumpHeight = 2.0f;				// Height of the player's jump

	//[Range(10.0f, 80.0f)]
	//public float maxWalkableSlope = 45.0f;		// NOT CURRENTLY IN USE

	private bool onGround = false;

	public bool OnGround
	{
		get{ return onGround; }
		set{ onGround = value; }
	}

	Rigidbody rb;
	CapsuleCollider capCollider;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody>();
		capCollider = GetComponent<CapsuleCollider>();

		// Prevent the player from being knocked over by walking into objects
		rb.freezeRotation = true;
		rb.useGravity = false;
		Debug.LogWarning("Using old script for player walking. Please attach the new script (FPC_Walk) and disable this script (FPC_Walk OLD)");
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		/* DESCRIPTION:
		 * Each Fixed Update call, this function calculates how the player object should
		 * move around the world, based on input and surrounding forces. */

		// Get the position at the bottom of the players collider (where the feet are)
		//Vector3 feetPosition = transform.position - new Vector3 (0, (capCollider.height / 2) + capCollider.radius, 0);
		//
		//// Spherecast to check all objects below the player's feet.
		//RaycastHit[] castHits = Physics.SphereCastAll (new Ray (feetPosition, Vector3.down), 
		//	capCollider.radius, 
		//	0.62f - capCollider.radius);
		//
		//foreach (RaycastHit hit in castHits)
		//{
		//	// Check everything under the player's feet
		//	float walkableSlopeNormal = Mathf.Sin(maxWalkableSlope);
		//	if (hit.normal.y >= walkableSlopeNormal)
		//	{
		//		Vector3 tangent = Vector3.Cross (transform.forward, hit.normal);
		//		Vector3 bitangent = Vector3.Cross (hit.normal, tangent);
		//
		//		Vector3 forwardForce = bitangent * Input.GetAxis ("Vertical");
		//		Vector3 strafeForce = bitangent * Input.GetAxis ("Horizontal");
		//
		//		//Vector3 targetVelocity = strafeForce + forwardForce;
		//		Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		//
		//		// Normalize the vector, apply rotation based on look angle and then apply speed
		//		targetVelocity = targetVelocity.normalized;
		//		targetVelocity = transform.TransformDirection (targetVelocity);
		//		targetVelocity *= speed;
		//
		//		// Find difference between current and target velocity
		//		Vector3 currentVelocity = rb.velocity;
		//		Vector3 deltaVelocity = (targetVelocity - currentVelocity);
		//
		//		// Prevent velocity from exceeding the max velocity change
		//		// This will prevent insane acceleration which would cause the player to go flying
		//		deltaVelocity.x = Mathf.Clamp(deltaVelocity.x, -maxVelocityChange, maxVelocityChange);
		//		deltaVelocity.z = Mathf.Clamp(deltaVelocity.z, -maxVelocityChange, maxVelocityChange);
		//		deltaVelocity.y = 0;
		//
		//		// Apply the force
		//		rb.AddForce (deltaVelocity, ForceMode.VelocityChange);
		//
		//		// Only allow player to jump if on the ground
		//		if (canJump && Input.GetButton ("Jump"))
		//		{
		//			rb.velocity = new Vector3 (currentVelocity.x, CalculateJumpSpeedVertical(), currentVelocity.z);
		//		}
		//
		//		break;
		//	}
		//}
		//
		//// Add manual gravity
		//rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

		// Calculate the force to be applied
		float forwardWalk = Input.GetAxis ("Vertical");
		float sideStrafe = Input.GetAxis ("Horizontal");
		Vector3 targetVelocity = new Vector3 (sideStrafe, 0, forwardWalk);
		targetVelocity = targetVelocity.normalized;

		// Normalize the vector then apply speed
		targetVelocity = transform.TransformDirection (targetVelocity);
		targetVelocity *= speed;

		Vector3 currentVelocity = rb.velocity;
		Vector3 deltaVelocity = (targetVelocity - currentVelocity);

		// Do not allow the force to exceed the maxVelocityChange
		deltaVelocity.x = Mathf.Clamp(deltaVelocity.x, -maxVelocityChange, maxVelocityChange);
		deltaVelocity.z = Mathf.Clamp(deltaVelocity.z, -maxVelocityChange, maxVelocityChange);
		deltaVelocity.y = 0;

		// Add the force
		rb.AddForce(deltaVelocity, ForceMode.VelocityChange);

		// Apply manual gravity
		onGround = checkOnGround ();
		if (!onGround)
			rb.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);

		// Only allow player to jump if on the ground
		if (onGround && canJump && Input.GetButton ("Jump"))
		{
			rb.velocity = new Vector3 (currentVelocity.x, CalculateJumpSpeedVertical(), currentVelocity.z);
		}

		//TODO MOVE THIS SHIZ
		if (Input.GetKeyDown ("escape"))
			Cursor.lockState = CursorLockMode.None;
	}

	bool checkOnGround()
	{
		/* Raycast from several points below the player to determine
		 * whether or not the player is on the ground. This is used
		 * for jumping, etc.
		 */

		float playerHeight = capCollider.bounds.size.y;
		float playerRadius = capCollider.radius * 0.8f;
		Vector3 playerFeetOffset = transform.position + (new Vector3 (0, -(playerHeight / 2), 0));
		float detectionDist = 0.38f;

		// Raycast from the center of the player's feet position
		if (Physics.Raycast (playerFeetOffset, Vector3.down, detectionDist))
			return true;

		// Linecast from all sides
		Vector3 detectionLength = new Vector3 (0, -detectionDist, 0);
		if (	Physics.Linecast (playerFeetOffset + new Vector3(playerRadius, 0, 0), playerFeetOffset + new Vector3(playerRadius, 0, 0) + detectionLength)
			||	Physics.Linecast (playerFeetOffset + new Vector3(-playerRadius, 0, 0), playerFeetOffset + new Vector3(-playerRadius, 0, 0) + detectionLength)
			||	Physics.Linecast (playerFeetOffset + new Vector3(0, 0, playerRadius), playerFeetOffset + new Vector3(0, 0, playerRadius) + detectionLength)
			||	Physics.Linecast (playerFeetOffset + new Vector3(0, 0, -playerRadius), playerFeetOffset + new Vector3(0, 0, -playerRadius) + detectionLength))
			return true;

		// All failed, the player is not grounded
		return false;
	}

	float CalculateJumpSpeedVertical()
	{
		return Mathf.Sqrt (jumpHeight * gravity * 1.5f);
	}
}


