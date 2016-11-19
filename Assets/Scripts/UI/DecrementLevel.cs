using UnityEngine;
using System.Collections;

public class DecrementLevel : MBAction {

	public override void Execute ()
	{
		LevelOrder.PlayerDeathBackLevel();
	}
}
