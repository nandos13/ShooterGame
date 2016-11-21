using UnityEngine;
using System.Collections;

public class SetTimeScale : MBAction {

	[Range (0, 5)]
	public float scale = 1;

	public override void Execute ()
	{
		Time.timeScale = scale;
	}
}
