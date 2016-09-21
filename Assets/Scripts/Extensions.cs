using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Added custom functionality for pre-existing Unity Engine classes.
 */

public static class Extensions {

	public static Transform Search (this Transform target, string transformName)
	{
		/* This function will search through all children of a specified transform
		 * and if a child with the specified name is found, returns that transform.
		 */
		return RecursiveSearch (target, transformName);
	}

	private static Transform RecursiveSearch (Transform target, string transformName)
	{
		/* Recursively searches through all children of a transform to find
		 * a child with the specified name.
		 */
		if (target.name == transformName)
			return target;
		foreach (Transform t in target) 
		{
			if (RecursiveSearch (t, transformName) != null)
				return RecursiveSearch (t, transformName);
		}
		return null;
	}

	public static List<T> GetComponentsAscending<T> (this Transform target)
	{
		/* Returns a list of all components of type T that exist in target
		 * and target's parent heirarchy.
		 */
		List<T> components = new List<T>();

		// Check target transform
		ListAddGetComponentsAscending (target, ref components);

		// Check all parents through family tree
		Transform targetParent = target.parent;
		while (targetParent != null)
		{
			ListAddGetComponentsAscending (targetParent, ref components);
			targetParent = targetParent.parent;
		}

		return components;
	}

	private static void ListAddGetComponentsAscending<T> (Transform target, ref List<T> list)
	{
		/* Adds all components of type T contained in target to the List<T> list */
		if (target)
		{
			T[] targetsComponents = target.GetComponents<T> ();
			foreach (T comp in targetsComponents)
				list.Add (comp);
		}
	}
}
