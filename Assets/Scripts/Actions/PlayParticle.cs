using UnityEngine;
using System.Collections;

public class PlayParticle : MBAction {

	public ParticleSystem particles;

	public override void Execute ()
	{
		if (particles)
		{
			if (collision.collider)
				particles.transform.position = collision.contacts[0].point;
			particles.Play();
		}
	}
}
