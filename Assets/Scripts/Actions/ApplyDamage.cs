using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Damage a component that is collided with.
 */

public class ApplyDamage : MBAction {

	public float Damage;
	public List<string> dmgTags = new List<string>();					// List of tags that are either ignored or exclusively-damaged
	public COLLISION_MODE dmgTagsMode = COLLISION_MODE.IgnoreSelected;

	public override void Execute ()
	{
		Health healthComponent = collision.transform.GetComponentAscendingImmediate<Health>(true);

		if (healthComponent)
		{
			// Should the hit deal damage or be ignored?
			bool dmgIgnore = false;
			if (dmgTagsMode == COLLISION_MODE.HitSelected)
				dmgIgnore = true;

			foreach (string str in dmgTags)
			{
				if (healthComponent.transform.tag == str)
				{
					if (dmgTagsMode == COLLISION_MODE.IgnoreSelected)
					{
						dmgIgnore = true;
						break;
					}
					else if (dmgTagsMode == COLLISION_MODE.HitSelected)
					{
						dmgIgnore = false;
						break;
					}
				}
			}

			if (!dmgIgnore)
				healthComponent.ApplyDamage(Damage);
		}
	}
}
