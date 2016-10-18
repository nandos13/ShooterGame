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
		if (spawnObject)
		{
			spawnObject = Instantiate (spawnObject) as GameObject;
			spawnObject.SetActive (false);
		}
	}

	public override void Execute ()
	{
		thisObject.SetActive (false);
		if (spawnObject)
		{
			spawnObject.SetActive (true);
			spawnObject.transform.position = thisObject.transform.position;
			spawnObject.transform.rotation = thisObject.transform.rotation;
		}
	}
}
