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

	public bool inheritRotation = true;
	public bool inheritScale = true;

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
		if (inheritRotation)
			spawnObject.transform.rotation = thisObject.transform.rotation;
		if (inheritScale)
			spawnObject.transform.localScale = thisObject.transform.localScale;

		if (! (thisObject.transform.parent == thisObject.transform) )
		{
			spawnObject.transform.parent = thisObject.transform.parent;
		}
	}
}
