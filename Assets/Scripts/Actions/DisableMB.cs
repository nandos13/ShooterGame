using UnityEngine;
using System.Collections;

public class DisableMB : MBAction {

	public MonoBehaviour script;

	public override void Execute ()
	{
		if (script)
			script.enabled = false;
	}
}
