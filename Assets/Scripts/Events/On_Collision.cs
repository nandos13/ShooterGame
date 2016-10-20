using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * On colliding with an object, executes a list of actions.
 */

public class On_Collision : MonoBehaviour {

	public List<string> collisionTags = new List<string>();			// List of tags that are either ignored or exclusively-hit
	public COLLISION_MODE mode = COLLISION_MODE.IgnoreSelected;
	public List<MBAction> Actions = new List<MBAction>();			// List of actions to execute on collision

	void OnCollisionEnter (Collision collision)
	{
		// Should this collision be ignored?
		bool ignoring = false;
		if (mode == COLLISION_MODE.HitSelected)
			ignoring = true;

		foreach (string str in collisionTags)
		{
			if (collision.transform.tag == str)
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

		// Run actions
		if (!ignoring)
		{
			foreach (MBAction action in Actions)
			{
				action.collision = collision;
				action.Execute();
			}
		}
	}
}
