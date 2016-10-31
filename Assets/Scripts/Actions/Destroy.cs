using UnityEngine;
using System.Collections;

public class Destroy : MBAction {

	[Range (0.0f, 60.0f)]
	public float waitTime = 0;

	public override void Execute()
	{
		StartCoroutine(dest());
	}

	private IEnumerator dest ()
	{
		yield return new WaitForSeconds (waitTime);
		Destroy (this);
	}
}
