using UnityEngine;
using System.Collections;

public class ApplyGravity : MonoBehaviour {

	public float gravity = 0.0f;			// Gravity in m/s^2
	private Rigidbody rb;

	void Start () 
	{
		if (gravity < 0.0f)
			gravity = 0.0f;

		rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate () 
	{
		if (rb)
			rb.AddForce (Vector3.down * gravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
	}
}
