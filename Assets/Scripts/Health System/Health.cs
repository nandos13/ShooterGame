using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * This class handles the health of an object or enemy including triggering
 * specified events upon taking damage or dying.
 */

public class Health : MonoBehaviour {

	public float MaxHealth = 100;				// The maximum health value for the object

	[Range(0, 100)]
	public float StartHealthPercent = 100;		// The object will spawn with this percentage of MaxHealth

	public List<MBAction> DeathScripts;			// Scripts run when the object reaches 0 health
	public List<MBAction> DamageScripts;		// Scripts run when the object takes damage

	private bool alive = true;					// Tracks if the object is alive
	public float currentHealth;				// The current health of the object
	private float lastHealth;					// Used to identify changes in health
	public float CurrentHealth
	{
		get { return currentHealth; }
	}


	// Use this for initialization
	void Start () 
	{
		currentHealth = MaxHealth * StartHealthPercent / 100.0f;
		lastHealth = currentHealth;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (alive) 
		{
			// Check if the object took damage 
			if (currentHealth < lastHealth)
			{
				// Run anything that happens on damage here
				if (DamageScripts.Count > 0) 
				{
					foreach (MBAction script in DamageScripts) 
					{
						if (script) 
						{
							script.Execute ();
						}
					}
				}

				// Store the new health value
				lastHealth = currentHealth;
			}

			// Check if the object dies this frame
			if (currentHealth <= 0.0f) 
			{
				alive = false;

				bool scriptsHaveBeenRun = false;

				// Run anything that happens on death here
				if (DeathScripts.Count > 0) 
				{
					foreach (MBAction script in DeathScripts) 
					{
						if (script) 
						{
							script.Execute ();
							scriptsHaveBeenRun = true;
						}
					}
				} 

				if (!scriptsHaveBeenRun) 
				{
					// No death script was specified
					Debug.Log ("Object " + name + " has no specified onDeath script");
				}
			}
		}
	}

	public void ApplyDamage (float dmg)
	{
		if (alive) 
		{
			currentHealth -= dmg;
			Debug.Log(transform.name + " took " + dmg + " damage. Health now at " + currentHealth);
		}
	}
}
