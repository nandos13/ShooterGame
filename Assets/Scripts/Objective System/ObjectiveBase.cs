using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Base class which stores all necessary information for a completeable objective.
 */

public abstract class ObjectiveBase : MonoBehaviour {

	public bool optional = false;
	public Image tick;

	private bool passed = false;

	public virtual bool Evaluate()
	{
		// Evaluate pass or fail
		if (!passed)
		{
			passed = inEvaluate();
		}

		return passed;
	}

	protected abstract bool inEvaluate();
}
