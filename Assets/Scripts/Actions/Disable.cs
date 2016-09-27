using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Disables the object when Execute is called.
 */

public class Disable : MBAction {

	public override void Execute()
	{
		Debug.Log("disabling");
		transform.gameObject.SetActive (false);
	}
}
