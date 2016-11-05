using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
public class FPC_Walk : MonoBehaviour {

	[Range (1, 1000)]
	public float mass = 100;
	[Range (1, 100)]
	public float movementSpeed = 15;
	[Range (0, 20)]
	public float jumpHeight = 10;
	[Range (0, 150)]
	public float gravity = 50;

	private Rigidbody rb;
	private CapsuleCollider col;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		if (rb)
		{
			rb.useGravity = false;
			rb.interpolation = RigidbodyInterpolation.Interpolate;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rb.angularDrag = 0;
			rb.drag = 0;
			rb.mass = mass;
		}

		col = GetComponent<CapsuleCollider> ();
	}

	void FixedUpdate () 
	{
		// Calculate the force to be applied based on player input
		float vert = Input.GetAxis ("Vertical");
		float horz = Input.GetAxis ("Horizontal");
		Vector3 targetVel = new Vector3 (horz, 0, vert);

		// Normalize to prevent movement speed variance
		targetVel.Normalize ();

		// Get direction in world space
		targetVel = transform.TransformDirection (targetVel);

		// Apply movement speed
		targetVel *= movementSpeed;

		// Calculate change in velocity
		Vector3 currVel = rb.velocity;
		Vector3 deltaVel = targetVel - currVel;
		deltaVel.y = 0;

		// Add force
		rb.AddForce (deltaVel, ForceMode.VelocityChange);

		// Prevent "bunnyhopping" issue when walking up slopes, etc.
		currVel = rb.velocity;
		if (currVel.y > 0 && grounded())
		{
			currVel.y = 0;
			rb.velocity = currVel;
		}

		applyGravity();

		handleJumping();
	}

	private void applyGravity ()
	{
		/* Handles the manual application of gravity if the player is grounded */

		if (!grounded())
			rb.AddForce (new Vector3 (0, -gravity, 0), ForceMode.Acceleration);
	}

	private bool grounded ()
	{
		/* Check if the player is currently standing on an object */
		// Spherecast down
		RaycastHit[] hits = 
			Physics.SphereCastAll (new Ray (transform.position, Vector3.down), col.radius, (col.bounds.extents.y) + 0.1f);

		if (hits.Length > 0)
		{
			// Disregard hits that are children of the player
			RaycastHit[] newHits = hits.IgnoreChildren (this.gameObject);

			if (newHits.Length > 0)
				return true;
		}

		return false;
	}

	private void handleJumping ()
	{
		if (grounded() && Input.GetButton("Jump"))
		{
			float jumpSpeed = Mathf.Sqrt (jumpHeight * gravity * 2);
			rb.velocity = new Vector3 (rb.velocity.x, jumpSpeed, rb.velocity.z);
		}
	}

	void OnDrawGizmosSelected ()
	{
		
	}
}
