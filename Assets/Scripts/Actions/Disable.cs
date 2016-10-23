using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Disables the object when Execute is called.
 */

public class Disable : MBAction {

	[Range (0.0f, 10.0f)]
	public float waitTime = 0;

	public override void Execute()
	{
		StartCoroutine(disable());
	}

	private IEnumerator disable ()
	{
		yield return new WaitForSeconds (waitTime);
		transform.gameObject.SetActive (false);
	}
}
