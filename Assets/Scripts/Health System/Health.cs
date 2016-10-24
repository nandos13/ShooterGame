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
	[Range(0.0f, 1.0f)]
	public float ImmunityTime;					// Object will become invulnerable to damage for x seconds after taking damage
	private bool immune = false;
	private float immuneCurHighestDmg = 0;		// Tracks the highest damage the player has taken during immunity

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
				Debug.Log(transform.name + " died");
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
					// Use custom function to get most immediate health scripts of children in each branch the heirarchy
					List<Health> childrenHealthComponents = transform.GetComponentsDescendingImmediate<Health>(false);
				
					// Kill all children
					foreach (Health h in childrenHealthComponents)
						h.Alive = false;
				}
			}
			alive = value;
		}
	}
	private float currentHealth;				// The current health of the object
	public float CurrentHealth
	{
		get { return currentHealth; }
	}
	public float CurrentHealthPct
	{
		get { return (currentHealth / MaxHealth) * 100; }
	}
	public float CurrentHealthDec
	{
		get { return (currentHealth / MaxHealth); }
	}


	// Use this for initialization
	void Awake () 
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

	private IEnumerator DisableImmunity ()
	{
		/* Puts the player in an immune state for a number of seconds.
		 */
		immune = true;
		yield return new WaitForSeconds (ImmunityTime);
		immune = false;
		immuneCurHighestDmg = 0;
	}

	private void doDamage (float dmg, bool relayDamage = true)
	{
		/* Directly applies damage to an object. All other classes should call the
		 * ApplyDamage method which then calls this method with proper values.
		 */
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

		// Should parent health objects also take damage?
		if (DamageParents && relayDamage)
		{
			// Use custom function to get health script of parents in the family tree heirarchy 
			Health parentHealthComponent = transform.GetComponentAscendingImmediate<Health>(false);

			if (parentHealthComponent)
			{
				parentHealthComponent.ApplyDamage(dmg);
			}
		}
	}

	public void ApplyDamage (float dmg, bool relayDamage = true)
	{
		/* Performs checks and calculates how much damage the object should take.
		 */
		// Is the object alive?
		if (alive) 
		{
			// Is the object in an immune state?
			if (immune)
			{
				/* If the object is immune, we will only apply the largest ammount of damage
				 * taken in this time frame. 
				 * Example, the player is shot by a low damage bullet. They take damage and become
				 * 'immune'. Then they are hit with an explosion while immune. If the explosion 
				 * damage is higer than the bullet, apply the damage minus the current damage taken.
				*/

				// Is this damage higher than the current highest while immune?
				if (dmg > immuneCurHighestDmg)
				{
					doDamage(dmg - immuneCurHighestDmg, relayDamage);
					immuneCurHighestDmg = dmg;
				}
			}
			else
			{
				// Make the player immune
				StartCoroutine(DisableImmunity ());
				immuneCurHighestDmg = dmg;

				// Apply the damage
				doDamage(dmg, relayDamage);
			}
		}
	}

	public void ApplyHeal (float h)
	{
		// Is the object alive?
		if (alive)
		{
			currentHealth += h;
		}
	}
}
