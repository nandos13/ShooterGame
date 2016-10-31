using UnityEngine;
using System.Collections;

public class SetEnabled : MBAction {

	public bool state = true;
	public GameObject target;

	public override void Execute()
	{
		if (target)
		{
			target.SetActive(state);
		}
	}
}
