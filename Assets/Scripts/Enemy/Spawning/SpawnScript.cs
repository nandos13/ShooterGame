using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Spawns an enemy on Execute(), using spawnPoint info from a specified spawnHandler
 */

public class SpawnScript : MBAction {

	public SpawnerHandler spawnHandler;
	public GameObject spawnObject;
	public bool aggroOnSpawn = true;

	void Start () 
	{
		if (!spawnHandler)
			Debug.LogWarning("No spawnHandler specified in SpawnScript on object '" + transform.name + "'. Please drag the spawnerHandler in.");

		if (!spawnObject)
			Debug.LogWarning("No object specified in SpawnScript on object '" + transform.name + "'. Please drag in the object to be spawned.");
	}

	public override void Execute () 
	{
		if (spawnHandler && spawnObject)
			spawnHandler.Spawn(spawnObject, aggroOnSpawn);
	}
}
