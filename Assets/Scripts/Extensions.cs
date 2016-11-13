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
				targetParent = targetParent.parent;
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
				targetParent = targetParent.parent;
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
			RecursiveAddChildComponents(target, ref components, false);
		}

		return components;
	}

	public static List<T> GetComponentsDescendingImmediate<T> (this Transform target, bool includeSelf)
	{
		/* Returns the components of type T that exists in the most immediate
		 * child in target's child heirarchy containing a T-component.
		 */
		List<T> components = new List<T>();

		// Check target transform
		if (includeSelf)
			ListAddGetComponents (target, ref components);

		// Check all children through family tree
		if (target.transform.childCount > 0)
		{
			RecursiveAddChildComponents(target, ref components, true);
		}

		return components;
	}

	private static void RecursiveAddChildComponents<T> (Transform target, ref List<T> list, bool exitOnFind)
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

			// Does the function stop looking through one tree deviation if something is found?
			if (!exitOnFind)
				RecursiveAddChildComponents(targetChild, ref list, true);
		}
	}

	public static void ApplyDamage (this Transform target, float dmg)
	{
		/* Finds the first instance of a health script on the target
		 * transform or one of its parents, and applies damage.
		 */

		Health h = target.GetComponentAscendingImmediate<Health>(true);

		// Was a health script found?
		if (h)
			h.ApplyDamage(dmg);
	}

	public static RaycastHit ApplyTagMask (this RaycastHit[] target, List<string> tags, COLLISION_MODE mode = COLLISION_MODE.IgnoreSelected)
	{
		/* Apply a tag mask to a list of RaycastHits and return the first
		 * instance of RaycastHit that should not be ignored, based on
		 * specified list "tags" and collision mode "mode"
		 */

		RaycastHit result = new RaycastHit();

		// Loop through each raycastHit in the array
		foreach (RaycastHit hit in target)
		{
			bool ignore = false;
			if (mode == COLLISION_MODE.HitSelected)
				ignore = true;

			foreach (string str in tags)
			{
				// Is the hit tag found in tags list?
				if (hit.transform.tag == str)
				{
					// Toggle bool ignore
					ignore = !ignore;
					break;
				}
			}

			// Is the hit being ignored?
			if (!ignore)
			{
				result = hit;
				break;
			}
		}

		// Return match
		return result;
	}

	public static RaycastHit ApplyTagMask (this RaycastHit[] target, string[] tags, COLLISION_MODE mode = COLLISION_MODE.IgnoreSelected)
	{
		/* Apply a tag mask to a list of RaycastHits and return the first
		 * instance of RaycastHit that should not be ignored, based on
		 * specified list "tags" and collision mode "mode"
		 */

		RaycastHit result = new RaycastHit();

		// Loop through each raycastHit in the array
		foreach (RaycastHit hit in target)
		{
			bool ignore = false;
			if (mode == COLLISION_MODE.HitSelected)
				ignore = true;

			foreach (string str in tags)
			{
				// Is the hit tag found in tags list?
				if (hit.transform.tag == str)
				{
					// Toggle bool ignore
					ignore = !ignore;
					break;
				}
			}

			// Is the hit being ignored?
			if (!ignore)
			{
				result = hit;
				break;
			}
		}

		// Return match
		return result;
	}

	public static RaycastHit[] IgnoreChildren (this RaycastHit[] target, GameObject parent)
	{
		/* Return an array of RaycastHit that does not contain any
		 * children of the specified GameObject "parent"
		 */

		List<Transform> children = (parent.GetComponentsInChildren<Transform> ()).ToList ();

		List<RaycastHit> newHitsList = new List<RaycastHit> ();

		foreach (RaycastHit hit in target)
		{
			bool ignore = false;

			foreach (Transform go in children)
			{
				if (go == hit.transform)
				{
					ignore = true;
					break;
				}
			}

			if (!ignore)
				newHitsList.Add(hit);
		}

		return newHitsList.ToArray();
	}

	public static T[] ToArray<T> (this List<T> target)
	{
		/* Convert a List of type T to an array containing type T */

		T[] result = new T[target.Count];

		for (int i = 0; i < result.Length; i++)
		{
			if (i < target.Count)
				result[i] = target[i];
		}

		return result;
	}

	public static List<T> ToList<T> (this T[] target)
	{
		/* Convert an array containing type T to a List of type T */

		List<T> result = new List<T>();

		foreach (T data in target)
		{
			result.Add(data);
		}

		return result;
	}
}
