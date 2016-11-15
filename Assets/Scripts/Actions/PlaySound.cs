using UnityEngine;
using System.Collections;

public class PlaySound : MBAction {

	public AudioSource source;
	public AudioClip sound;

	void Start ()
	{
		if (source)
			source.playOnAwake = false;
	}

	public override void Execute () 
	{
		if (source && sound)
			source.PlayOneShot(sound);
	}
}
