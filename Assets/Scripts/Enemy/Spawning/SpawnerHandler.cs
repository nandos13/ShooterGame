using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * This script contains information about spawn locations in the level. Each level must have an 
 * instance of this script somewhere in the heirarchy for enemy spawns to work.
 */

public class SpawnerHandler : MonoBehaviour {

	public static SpawnerHandler instance;							// Only allow a single instance of this script to exist
	public List<Transform> spawnPoints = new List<Transform>();		// List of spawn points

	private GameObject player;										// Reference to player, used for aggro on spawn

	void Start () 
	{
		if (!instance)
			instance = this;
		else
		{
			Destroy(this);
			Debug.LogWarning("More than one instance of class SpawnerHandler exists. Deleting duplicate instance.");
		}

		if (spawnPoints.Count <= 0)
			Debug.LogWarning("No spawn points specified in SpawnHandler script. No enemies will spawn.");

		// Get player reference
		player = GameObject.Find ("Player");
	}

	public void Spawn (GameObject obj, bool aggroPlayer)
	{
		/* Spawns a specified object at a random spawn point in the level */

		if (obj  && spawnPoints.Count > 0)
		{
			// Instantiate new instance of obj
			GameObject newObj = Instantiate (obj) as GameObject;

			// Choose random spawn point
			int i = pickSpawn ();

			newObj.transform.position = spawnPoints[i].position;

			if (aggroPlayer)
			{
				// Get attached navmesh agent and set destination to player position
				NavMeshAgent navAgent = newObj.GetComponent<NavMeshAgent>();
				if (navAgent)
				{
					if (player)
						navAgent.destination = player.transform.position;
					else
						Debug.LogWarning("Could not find reference to player.");
				}
				else
					Debug.LogWarning("Spawned enemy does not have an attached navmesh component and can't aggro the player.");
			}
		}
	}

	private int pickSpawn ()
	{
		/* Picks a random spawn point from Transform array spawnPoints */

		removeBlankSpawns();
		if (spawnPoints.Count >= 0)
		{
			int i = -1;
			while (i < 0)
			{
				int index = Random.Range (0, spawnPoints.Count);
				// Check transform exists
				if (spawnPoints[index] != null)
					i = index;
			}
			return i;
		}
		else
			return -1;
	}

	private void removeBlankSpawns ()
	{
		/* Iterates through all spawnPoints and removes any null transforms */

		for (int i = 0; i < spawnPoints.Count; i++)
		{
			if (spawnPoints[i] == null)
			{
				// Remove this index
				spawnPoints.RemoveAt(i);

				// Set iterator back so nothing is skipped
				i--;
			}
		}
	}
}
