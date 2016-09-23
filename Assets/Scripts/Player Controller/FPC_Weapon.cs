using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * 
 */

[RequireComponent(typeof(Weapon_Base_Script))]
public class FPC_Weapon : MonoBehaviour 
{

	Weapon_Base_Script weapon;

	// Use this for initialization
	void Start () 
	{
		weapon = GetComponent<Weapon_Base_Script> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetAxisRaw ("Fire1") > 0) 
		{
			if (weapon) 
			{
				weapon.Execute ();
			}
		}
	}
}
