using UnityEngine;
using System.Collections;

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

	private NavMeshAgent navAgent;

	void Start () 
	{
		navAgent = GetComponent<NavMeshAgent>();
	}

	void Update () 
	{
		if (navAgent && target && eyes)
		{
			// Check target is within range
			if (Vector3.Distance (target.transform.position, transform.position) < trackingRange)
			{
				// Linecast for line of sight
				RaycastHit hit = new RaycastHit();
				if (Physics.Linecast(eyes.position, target.transform.position, out hit))
				{
					if (hit.collider.gameObject == target)
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
}
