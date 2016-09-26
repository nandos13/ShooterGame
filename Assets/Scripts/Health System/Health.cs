using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * This class handles the health of an object or enemy including triggering
 * specified events upon taking damage or dying.
 */

public class Health : MonoBehaviour {

	public bool DamageParents = false;			// Will damaging the object also apply damage to any parents with the health script?
	public bool KillChildrenOnDeath = false;	// Will killing the object also kill any child objects with the health script?
	public float MaxHealth = 100;				// The maximum health value for the object

	[Range(0, 100)]
	public float StartHealthPercent = 100;		// The object will spawn with this percentage of MaxHealth

	public List<MBAction> DeathScripts = new List<MBAction>();			// Scripts run when the object reaches 0 health
	public List<MBAction> DamageScripts;		// Scripts run when the object takes damage

	private bool alive = true;					// Tracks if the object is alive
	public bool Alive
	{
		get { return alive;}
		set 
		{
			if (value == false && alive == true)
			{
				// Run anything that happens on death here
				if (DeathScripts.Count > 0) 
				{
					foreach (MBAction script in DeathScripts) 
					{
						if (script) 
						{
							script.Execute ();
						}
					}
				} 
				else 
				{
					// No death script was specified
					Debug.Log ("Object " + name + " has no specified onDeath script");
				}

				// Should child health objects also die upon death of this object?
				if (KillChildrenOnDeath)
				{
					// Use custom function to get all health scripts of children in the heirarchy
					List<Health> childrenHealthComponents = transform.GetComponentsDescending<Health>(false);
				
					// Kill all children
					foreach (Health h in childrenHealthComponents)
						h.Alive = false;
				}
			}
			alive = value;
		}
	}
	public float currentHealth;				// The current health of the object
	public float CurrentHealth
	{
		get { return currentHealth; }
	}


	// Use this for initialization
	void Start () 
	{
		if (MaxHealth < 0.0f) 
		{
			MaxHealth = 0.0f;
			currentHealth = 0.0f;
		}
		else
		{
			currentHealth = MaxHealth * StartHealthPercent / 100.0f;
		}
	}

	public void ApplyDamage (float dmg)
	{
		if (alive) 
		{
			currentHealth -= dmg;
			//Debug.Log(transform.name + " took " + dmg + " damage. Health now at " + currentHealth);

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

			// Check if the object dies this frame
			if (currentHealth <= 0.0f)
				Alive = false;
			//if (currentHealth <= 0.0f) 
			//{
			//	alive = false;
			//
			//	// Run anything that happens on death here
			//	if (DeathScripts.Count > 0) 
			//	{
			//		foreach (MBAction script in DeathScripts) 
			//		{
			//			if (script) 
			//			{
			//				script.Execute ();
			//			}
			//		}
			//	} 
			//	else 
			//	{
			//		// No death script was specified
			//		Debug.Log ("Object " + name + " has no specified onDeath script");
			//	}
			//
			//	// Should child health objects also die upon death of this object?
			//	if (KillChildrenOnDeath)
			//	{
			//		// Use custom function to get all health scripts of children in the heirarchy
			//		List<Health> childrenHealthComponents = transform.GetComponentsDescending<Health>(false);
			//
			//		// Kill all children
			//		foreach (Health h in childrenHealthComponents)
			//			h.Alive = false;
			//	}
			//}

			// Should parent health objects also take damage?
			if (DamageParents)
			{
				// Use custom function to get health script of parents in the family tree heirarchy 
				Health parentHealthComponent = transform.GetComponentAscendingImmediate<Health>(false);

				if (parentHealthComponent)
				{
					parentHealthComponent.ApplyDamage(dmg);
				}
			}
		}
	}
}
