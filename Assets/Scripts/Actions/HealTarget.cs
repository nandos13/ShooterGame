using UnityEngine;
using System.Collections;

public class HealTarget : MBAction {

	public Health target;
	public float value;

	public override void Execute () 
	{
		if (target)
			target.ApplyHeal (value);
	}
}
