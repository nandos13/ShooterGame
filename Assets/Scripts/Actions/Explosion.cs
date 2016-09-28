using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MBAction {

	[Range(0.0f, 50.0f)]
	public float ExplosiveForce = 10;
	[Range(0.0f, 10.0f)]
	public float JumpModifier = 0.5f;
	[Range(0.0f, 100.0f)]
	public float Radius = 20;
	public float Damage = 14;
	public List<string> CollisionTags = new List<string>();	// A list of collisions to ignore
	public COLLISION_MODE mode = COLLISION_MODE.IgnoreSelected;

	public override void Execute ()
	{
		// Get all colliders within Radius of the transform
		Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

		// We need this boolean to decide which colliders the explosion is added to
		bool ignoring = false;
		if (mode == COLLISION_MODE.HitSelected)
			ignoring = true;

		// Apply the explosion to any colliders that arent being ignored
		foreach (Collider col in colliders)
		{
			// Reset the boolean for each collider
			if (mode == COLLISION_MODE.IgnoreSelected)
				ignoring = false;
			else
				ignoring = true;
			
			// Should we apply the explosion to this collider?
			foreach (string str in CollisionTags)
			{
				if (col.transform.tag == str)
				{
					if (mode == COLLISION_MODE.IgnoreSelected)
					{
						ignoring = true;
						break;
					}
					else if (mode == COLLISION_MODE.HitSelected)
					{
						ignoring = false;
						break;
					}
				}
			}

			if (!ignoring)
			{
				// Apply the explosion here
				Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
				Health h = col.gameObject.GetComponent<Health>();

				if (rb)
					rb.AddExplosionForce (ExplosiveForce, transform.position, Radius, JumpModifier, ForceMode.Impulse);

				if (h && Damage != 0)
				{
					// Calculate the damage based on the distance from the explosion
					float damageMultiplier = Vector3.Distance (transform.position, col.transform.position) / Radius;

					h.ApplyDamage (Damage * damageMultiplier);
				}
					
				// TODO: Add damage based on force
			}
		}
	}
}
