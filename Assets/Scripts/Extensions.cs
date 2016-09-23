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

	public static T GetComponentAscendingImmediate<T> (this Transform target, bool includeSelf)
	{
		/* Returns the most immediate single component contained by a transform
		 * in target's parent heirarchy
		 */

		// Check through parents in family tree
		Transform targetParent = target.parent;
		if (includeSelf)
			targetParent = target;
		while (true)
		{
			if (targetParent != null)
			{
				T targetsFirstComponent = targetParent.GetComponent<T> ();
				if (targetsFirstComponent != null)
					return targetsFirstComponent;
			}
			else {break;}
		}

		return default(T);
	}

	public static List<T> GetComponentsAscendingImmediate<T> (this Transform target, bool includeSelf)
	{
		/* Returns the components of type T that exists in the most immediate
		 * parent in target's parent heirarchy containing a T-component.
		 */
		List<T> components = new List<T>();

		// Check through parents in family tree
		Transform targetParent = target.parent;
		if (includeSelf)
			targetParent = target;
		while (true)
		{
			if (targetParent != null)
			{
				T[] targetsComponents = targetParent.GetComponents<T> ();
				if (targetsComponents.Length > 0)
				{
					foreach (T comp in targetsComponents)
					{
						components.Add (comp);
					}
					return components;
				}
			}
			else {break;}
		}

		return null;
	}

	public static List<T> GetComponentsAscending<T> (this Transform target, bool includeSelf)
	{
		/* Returns a list of all components of type T that exist in target
		 * and target's parent heirarchy.
		 */
		List<T> components = new List<T>();

		// Check target transform
		if (includeSelf)
			ListAddGetComponents (target, ref components);

		// Check all parents through family tree
		Transform targetParent = target.parent;
		while (targetParent != null)
		{
			ListAddGetComponents (targetParent, ref components);
			targetParent = targetParent.parent;
		}

		return components;
	}

	private static void ListAddGetComponents<T> (Transform target, ref List<T> list)
	{
		/* Adds all components of type T contained in target to the List<T> list */
		if (target)
		{
			T[] targetsComponents = target.GetComponents<T> ();
			foreach (T comp in targetsComponents)
				list.Add (comp);
		}
	}

	public static List<T> GetComponentsDescending<T> (this Transform target, bool includeSelf)
	{
		/* Returns a list of all components of type T that exist in target
		 * and target's children.
		 */
		List<T> components = new List<T>();

		// Check target transform
		if (includeSelf)
			ListAddGetComponents (target, ref components);

		// Check all children through family tree
		if (target.transform.childCount > 0)
		{
			RecursiveAddChildComponents(target, ref components);
		}

		return components;
	}

	private static void RecursiveAddChildComponents<T> (Transform target, ref List<T> list)
	{
		/* Recursively searches through all children of a transform and adds
		 * any any contained components of type T to a list.
		 */
		List<Transform> children = new List<Transform>();

		// Get an array of all children in the current target
		for (int i = 0; i < target.childCount; i++)
		{
			children.Add(target.GetChild(i));
		}

		// Get the components contained in each child, then recursively check all children of the target child
		foreach (Transform targetChild in children)
		{
			T[] targetsComponents = targetChild.GetComponents<T> ();
			foreach (T comp in targetsComponents)
				list.Add(comp);

			RecursiveAddChildComponents(targetChild, ref list);
		}
	}
}
