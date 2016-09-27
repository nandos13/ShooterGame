using UnityEngine;
using System.Collections;
using UnityEditor;

/* DESCRIPTION:
 * This scipt is a custom inspector which overrides the properties inspector for the
 * Weapon_Base_Script script. This allows the inspector to hide or show particular
 * variables based on the previously selected variables (weapon type, etc).
 */

[CustomEditor(typeof(Weapon_Base_Script))]
public class Weapon_Properties_Editor : Editor {

	public override void OnInspectorGUI ()
	{
		Weapon_Base_Script script = (Weapon_Base_Script)target;

		EditorGUILayout.Space();
		script.shotOrigin = (Transform)EditorGUILayout.ObjectField ("Shot Origin Point:", script.shotOrigin, typeof(Transform), true);
		EditorGUILayout.HelpBox ("All projectiles are fired in the forward position (blue axis). Please ensure the transform is rotated appropriately", MessageType.Info);

		EditorGUILayout.Space();
		script.muzzleFlash = (ParticleSystem)EditorGUILayout.ObjectField ("Muzzle Flash:", script.muzzleFlash, typeof(ParticleSystem), true);

		EditorGUILayout.Space();
		script.type = (WEAPON_TYPE)EditorGUILayout.EnumPopup ("Weapon Type:", script.type);

		EditorGUILayout.Space();

		// Show the correct properties based on Weapon Type
		switch (script.type) 
		{
		case WEAPON_TYPE.Beam:
			{
				// Show relevant variables for BEAM Weapon
				EditorGUILayout.LabelField ("Beam Weapon Specifics:", EditorStyles.boldLabel);

				script.HeatPerSecond = EditorGUILayout.Slider ("Heat Per Second:", script.HeatPerSecond, 1.0f, 100.0f);
				script.HeatFalloff = EditorGUILayout.Slider ("Heat Fall Off:", script.HeatFalloff, 1.0f, 100.0f);
				script.TimeBeforeCooldown = EditorGUILayout.Slider ("Time To Cool:", script.TimeBeforeCooldown, 0.0f, 3.0f);

				// Spacer
				EditorGUILayout.Space();

				break;
			}
		case WEAPON_TYPE.Bullet:
			{
				// Show relevant variables for BULLET Weapon
				EditorGUILayout.LabelField ("Bullet Weapon Specifics:", EditorStyles.boldLabel);

				script.hitscan = EditorGUILayout.Toggle ("Use Hitscan:", script.hitscan);
				if (!script.hitscan)
				{
					// Show physical bullet options
					script.bulletProjectile = (GameObject)EditorGUILayout.ObjectField ("Projectile:", script.bulletProjectile, typeof(Object), false);
					script.bulletForce = EditorGUILayout.IntSlider ("Muzzle Velocity:", script.bulletForce, 20, 150);
					script.DespawnBulletAfter = EditorGUILayout.Slider ("Despawn Time:", script.DespawnBulletAfter, 1.0f, 10.0f);
				}

				break;
			}
		case WEAPON_TYPE.Launcher:
			{
				// Show relevant variables for LAUNCHER Weapon
				EditorGUILayout.LabelField ("Launcher Weapon Specifics:", EditorStyles.boldLabel);

				break;
			}
		case WEAPON_TYPE.Pulse:
			{
				// Show relevant variables for PULSE Weapon
				EditorGUILayout.LabelField ("Pulse Weapon Specifics:", EditorStyles.boldLabel);

				break;
			}
		default:
			{
				break;
			}
		}

		// Show general variables
		EditorGUILayout.LabelField ("General Gun Properties:", EditorStyles.boldLabel);

		// Show ammo count variables (does not apply to beam weapons)
		if (script.type != WEAPON_TYPE.Beam) 
		{
			script.BottomlessClip = EditorGUILayout.Toggle ("Bottomless Clip:", script.BottomlessClip);

			// Clip size and maximum ammo variables only show if the weapon is not bottomless clip
			if (!script.BottomlessClip) 
			{
				script.ClipSize = (uint)EditorGUILayout.Slider ("Clip Size:", script.ClipSize, 1, 300);
				script.UnlimitedAmmo = EditorGUILayout.Toggle ("Unlimited Ammo:", script.UnlimitedAmmo);

				// Starting ammo variable is only shown if the weapon does not have unlimited ammo
				if (!script.UnlimitedAmmo) 
				{
					script.StartingAmmo = (uint)EditorGUILayout.Slider ("Starting Ammo:", script.StartingAmmo, 1, 500);
				}
			}

			// Spacer
			EditorGUILayout.Space();

			// Show damage property as "Damage" (not "Damage/Second") for non Beam Weapons
			script.Damage = EditorGUILayout.Slider ("Damage:", script.Damage, 0.1f, 300.0f);

			// Show shot speed for non Beam Weapons
			script.SpeedRPM = EditorGUILayout.Slider ("Shot Speed (rpm):", script.SpeedRPM, 20.0f, 1500.0f);
			script.Speed = script.SpeedRPM / 60;

			// Show spread for non Beam Weapons
			script.Spread = EditorGUILayout.Slider ("Spread:", script.Spread, 0.0f, 10.0f);
		}
		else
		{
			// Show damage property as "Damage/Second" for Beam Weapons
			script.Damage = EditorGUILayout.Slider ("Damage/Second:", script.Damage, 0.1f, 1000.0f);
		}
	}
}
