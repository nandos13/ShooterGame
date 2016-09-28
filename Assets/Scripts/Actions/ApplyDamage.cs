using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Damage a component that is collided with.
 */

public class ApplyDamage : MBAction {

	public float Damage;

	public override void Execute ()
	{
		collision.transform.ApplyDamage(Damage);
	}
}
