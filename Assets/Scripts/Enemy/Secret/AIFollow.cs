using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Simple script to handle enemy movement using a navmesh agent.
 */

[RequireComponent (typeof(NavMeshAgent))]
public class AIFollow : MonoBehaviour {

	[Range (0.0f, 500.0f)]
	public float trackingRange = 50.0f;
	[Range (0.0f, 30.0f)]
	public float closeRange = 5.0f;
	public Transform eyes;
	public GameObject target;
	public List<string>seeThroughTags = new List<string>();

	private NavMeshAgent navAgent;

	void Start () 
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Update () 
	{
		if (!Options.Paused && navAgent && target && eyes)
		{
			// Check target is within range
			if (Vector3.Distance (target.transform.position, transform.position) < trackingRange)
			{
				// Raycast for line of sight
				float distToTarget = Vector3.Distance (eyes.transform.position, target.transform.position);
				RaycastHit hit = new RaycastHit();
				RaycastHit[] hits = Physics.RaycastAll (eyes.position, (target.transform.position - eyes.position), distToTarget);
				if (hits.Length > 0)
				{
					// Ignore specified tags and get first raycastHit that should be visible
					hit = hits.ApplyTagMask (seeThroughTags, COLLISION_MODE.IgnoreSelected);

					if (hitIsTarget(hit))
					{
						navAgent.destination = target.transform.position;
					}
				}

				// Is the Agent within range to stop walking?
				if (Vector3.Distance (target.transform.position, transform.position) < closeRange)
				{
					navAgent.destination = transform.position;
				}
			}
		}
	}

	protected bool hitIsTarget (RaycastHit hit)
	{
		if (hit.collider)
		{
			if (hit.collider.gameObject == target)
				return true;
		}
		return false;
	}
}
