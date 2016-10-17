using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Basic example of a script which inherits from the MBAction class.
 * 
 * This script will simply add force to it's parent object when the Execute ()
 * function is called.
 */

public class ReplaceObject : MBAction {

	private GameObject thisObject;
	public GameObject spawnOnDead;

	void Start () 
	{
		thisObject = gameObject;
		if (spawnOnDead)
		{
			spawnOnDead = Instantiate (spawnOnDead) as GameObject;
			spawnOnDead.SetActive (false);
		}
	}

	public override void Execute ()
	{
		thisObject.SetActive (false);
		if (spawnOnDead)
		{
			spawnOnDead.SetActive (true);
			spawnOnDead.transform.position = thisObject.transform.position;
		}
	}
}
