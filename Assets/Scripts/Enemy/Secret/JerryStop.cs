using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * WARNING, ULTRA SECRET!
 * DO NOT READ!
 * PLEASE!
 * Uber secret script for jerry the cactus. He will follow you when you aren't looking at him.
 * We all love Jerry.
 */

public class JerryStop : MonoBehaviour {

	private Camera cam;
	private NavMeshAgent agent;

	void Start () 
	{
		cam = Camera.main;
		agent = GetComponent<NavMeshAgent>();
	}

	void OnWillRenderObject ()
	{
		if (agent)
		{
			// Do not stop Jerry's movement if the scene camera is looking at him
			#if UNITY_EDITOR
			if (Camera.current.name == "SceneCamera")
				return;
			#endif

			// Stop Jerry from moving, because THE PLAYER CAN SEE HIM!
			agent.destination = transform.position;
			agent.velocity = Vector3.zero;
		}
	}
}
