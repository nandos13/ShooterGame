using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectButton : MBAction {

	public Selectable btn;

	public override void Execute ()
	{
		if (btn)
			btn.Select();
	}
}
