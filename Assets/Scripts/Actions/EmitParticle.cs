using UnityEngine;
using System.Collections;

public class EmitParticle : MBAction {

	public ParticleSystem particles;
	public uint amount = 0;
	public bool inheritTransformPos = false;

	public override void Execute ()
	{
		if (particles)
		{
			if (inheritTransformPos)
				particles.transform.position = transform.position;
			if (collision.collider)
				particles.transform.position = collision.contacts[0].point;
			particles.Emit((int)amount);
		}
	}
}
