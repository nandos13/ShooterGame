using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Basic example of a script which inherits from the MBAction class.
 * 
 * This script will simply add force to it's parent object when the Execute ()
 * function is called.
 */

public class testAction2 : MBAction {

	private GameObject thisObject;
	public GameObject spawnOnDead;

	// Use this for initialization
	void Start () 
	{
		thisObject = gameObject;
		spawnOnDead = Instantiate (spawnOnDead) as GameObject;
		spawnOnDead.SetActive (false);
	}

	public override void Execute ()
	{
		thisObject.SetActive (false);
		spawnOnDead.SetActive (true);
		spawnOnDead.transform.position = thisObject.transform.position;

		Debug.Log ("testAction2 is being executed");
	}
}
