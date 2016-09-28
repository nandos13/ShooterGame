using UnityEngine;
using System.Collections;

public class EmitParticle : MBAction {

	public ParticleSystem particles;
	public uint amount = 0;

	public override void Execute ()
	{
		if (particles)
		{
			particles.Emit((int)amount);
		}
	}
}
