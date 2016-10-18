using UnityEngine;
using System.Collections;

public class ToggleEnabled : MBAction {

	public override void Execute()
	{
		if(transform.gameObject.activeSelf)
			transform.gameObject.SetActive(false);
		else
			transform.gameObject.SetActive(true);
	}
}
