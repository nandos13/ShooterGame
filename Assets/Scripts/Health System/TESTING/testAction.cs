using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Basic example of a script which inherits from the MBAction class.
 * 
 * This script will simply add force to it's parent object when the Execute ()
 * function is called.
 */

public class testAction : MBAction {

	Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public override void Execute ()
	{
		if (rb)
			rb.AddForce (new Vector3 (100, 100, 100), ForceMode.Force);
		
		Debug.Log ("testAction is being executed");
	}
}
