using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * This script can be added once or multiple times to an object to
 * allow it to do different things upon reaching a specified health value.
 * The enum variable "condition" allows control over whether this code can
 * only be run upon taking damage, upon healing, or both.
 * Simply add the OnHealthReachesValue script to an object, then drag another
 * script which inherits from the MBAction class into the "On Health Value Script" 
 * property in the inspector.
 */

public enum HealthChangeConditions{ Damaged, Healed, Both }

[RequireComponent (typeof(Health))]	// Requires the object has the Health script applied
public class OnHealthReachesValue : MonoBehaviour {

	public float HealthValue = 0.0f;			// The health value condition which will trigger the script
	public MBAction onHealthValueScript;		// Script to be executed when health reaches specified value

	public HealthChangeConditions condition = HealthChangeConditions.Damaged;	// Should this code trigger when the object gains health, loses is, or both?

	private float currentHealth;				// Tracks health for changes
	private Health healthObject;				// References the health script of the parent object

	// Use this for initialization
	void Start () 
	{
		healthObject = GetComponent<Health> ();
		currentHealth = healthObject.CurrentHealth;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (healthObject.CurrentHealth != currentHealth)
		{
			// Health has changed, check if HealthValue was reached or passed
			switch (condition)
			{

			case HealthChangeConditions.Damaged:
				{
					// Only trigger if the object lost health and met the value
					if (currentHealth > HealthValue && healthObject.CurrentHealth <= HealthValue)
						onHealthValueScript.Execute ();

					break;
				}

			case HealthChangeConditions.Healed:
				{
					// Only trigger if the object gained health and met the value
					if (currentHealth < HealthValue && healthObject.CurrentHealth >= HealthValue)
						onHealthValueScript.Execute ();

					break;
				}

			case HealthChangeConditions.Both:
				{
					// Trigger if the object met the value
					if (	(currentHealth > HealthValue && healthObject.CurrentHealth <= HealthValue)
						|| 	(currentHealth < HealthValue && healthObject.CurrentHealth >= HealthValue) )
						onHealthValueScript.Execute ();

					break;
				}

			}

			// Store the new health value
			currentHealth = healthObject.CurrentHealth;
		}
	}
}
