using UnityEngine;
using System.Collections;

public class SetCurrentLevel : MBAction {

	public int index = 0;

	public override void Execute ()
	{
		LevelOrder.SetCurrentLevel (index);
	}
}
