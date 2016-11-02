using UnityEngine;
using System.Collections;

public class HealTarget : MBAction {

	public Health target;
	public float value;
	public bool healPlayer = true;

	void Start ()
	{
		if (healPlayer)
		{
			GameObject p = GameObject.Find("Player");
			if (p)
			{
				Health h = p.GetComponent<Health>();
				if (h)
				{
					target = h;
				}
			}
		}
	}

	public override void Execute () 
	{
		if (target)
			target.ApplyHeal (value);
	}
}
