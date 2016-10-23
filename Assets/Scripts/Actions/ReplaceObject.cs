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
	public GameObject spawnObject;

	void Start () 
	{
		thisObject = gameObject;
	}

	public override void Execute ()
	{
		thisObject.SetActive (false);

		spawnObject = Instantiate (spawnObject) as GameObject;
		spawnObject.SetActive (true);
		spawnObject.transform.position = thisObject.transform.position;
		spawnObject.transform.rotation = thisObject.transform.rotation;

		if (! (thisObject.transform.parent == this.transform))
		{
			spawnObject.transform.parent = thisObject.transform.parent;
		}
	}
}
