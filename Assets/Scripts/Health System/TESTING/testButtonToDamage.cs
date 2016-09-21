using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Quick example script to damage the parent object. Just for proof of concept */

[RequireComponent (typeof(Health))]
public class testButtonToDamage : MonoBehaviour {

	private Health health;

	// Use this for initialization
	void Start () {
		health = GetComponent<Health> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown ("e")) 
		{
			if (health) 
			{
				health.ApplyDamage (2);
			}
		}
	}
}
