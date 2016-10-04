using UnityEngine;
using System.Collections;

public class EmitParticle : MBAction {

	public ParticleSystem particles;
	public uint amount = 0;

	public override void Execute ()
	{
		if (particles)
		{
			if (collision.collider)
				particles.transform.position = collision.contacts[0].point;
			particles.Emit((int)amount);
		}
	}
}
