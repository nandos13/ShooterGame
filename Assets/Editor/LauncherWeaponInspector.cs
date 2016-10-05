using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/* DESCRIPTION:
 * This script is a cunstom inspector which overrides the properties inspector for the
 * LauncherWeapon script. This allows the inspector to hide or show particular variables
 * based on previously selected variables. Also allows a custom layout and an
 * over all nicer looking inspector.
 */

[CustomEditor(typeof(LauncherWeapon))]
public class LauncherWeaponInspector : Editor {

	public bool showSoundList = false;

	public override void OnInspectorGUI ()
	{
		LauncherWeapon script = (LauncherWeapon)target;

		/* GENERAL SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		script.shotOrigin = (Transform)EditorGUILayout.ObjectField ("Shot Origin Point:", script.shotOrigin, typeof(Transform), true);
		script.audioSrc = (AudioSource)EditorGUILayout.ObjectField ("Audio Source:", script.audioSrc, typeof(AudioSource), true);

		/* VISUAL SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Visual:", EditorStyles.boldLabel);
		script.muzzleFlash = (ParticleSystem)EditorGUILayout.ObjectField ("Muzzle Flash:", script.muzzleFlash, typeof(ParticleSystem), true);
		script.muzzleParticles = (uint)EditorGUILayout.Slider ("Muzzle Emission:", script.muzzleParticles, 0, 100);
		script.hitEffect = (ParticleSystem)EditorGUILayout.ObjectField ("Hit Effect:", script.hitEffect, typeof(ParticleSystem), true);
		script.hitParticles = (uint)EditorGUILayout.Slider ("Hit Emission:", script.hitParticles, 0, 100);

		/* AUDIO SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Audio:", EditorStyles.boldLabel);
		showSoundList = EditorGUILayout.Foldout(showSoundList, "Fire Sounds");
		if (showSoundList)
		{
			for (int i = 0; i < script.shotSound.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("-", GUILayout.Width(23)))
					script.shotSound.RemoveAt(i);
				else
					script.shotSound[i] = (AudioClip)EditorGUILayout.ObjectField ("", script.shotSound[i], typeof(AudioClip), false);

				EditorGUILayout.EndHorizontal();
			}
			if (script.shotSound.Count > 0)
				EditorGUILayout.Space();
			if (GUILayout.Button("+", GUILayout.Width(23)))
			{
				script.shotSound.Add(new AudioClip());
			}
		}
		EditorGUILayout.Space();

		/* GUN SETTINGS */
		EditorGUILayout.LabelField ("Standard Properties:", EditorStyles.boldLabel);
		script.bottomlessClip = EditorGUILayout.Toggle ("Bottomless Clip:", script.bottomlessClip);
		// Clip size and maximum ammo variables only show if the weapon is not bottomless clip
		if (!script.bottomlessClip) 
		{
			script.clipSize = (uint)EditorGUILayout.Slider ("   Clip Size:", script.clipSize, 1, 300);
			script.unlimitedAmmo = EditorGUILayout.Toggle ("   Unlimited Ammo:", script.unlimitedAmmo);

			// Starting ammo variable is only shown if the weapon does not have unlimited ammo
			if (!script.unlimitedAmmo) 
			{
				script.startingAmmo = (uint)EditorGUILayout.Slider ("      Starting Ammo:", script.startingAmmo, 1, 500);
			}
		}
		script.damage = EditorGUILayout.Slider ("Damage:", script.damage, 0.1f, 300.0f);
		script.speedRPM = EditorGUILayout.Slider ("Shot Speed (rpm):", script.speedRPM, 20.0f, 1500.0f);
		script.speed = script.speedRPM / 60;
		script.spread = EditorGUILayout.Slider ("Spread:", script.spread, 0.0f, 10.0f);
		EditorGUILayout.Space();

		/* BULLET WEAPON SETTINGS */
		EditorGUILayout.LabelField ("Launcher Weapon Specifics:", EditorStyles.boldLabel);
		GameObject proj = (GameObject)EditorGUILayout.ObjectField ("Projectile:", script.missileProjectile, typeof(Object), false);
		// Do not allow a prefab without a rigidbody
		if (proj)
		{
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			if (!rb)
			{
				proj = null;
				Debug.LogError("Error: You tried to add a missile Projectile without a rigidbody component! Please ensure the prefab has a rigidbody attached.");
			}
			else
				script.missileProjectile = proj;
		}
		script.muzzleVelocity = EditorGUILayout.IntSlider ("Muzzle Velocity:", script.muzzleVelocity, 1, 20);
		script.despawnAfter = EditorGUILayout.Slider ("Despawn Time:", script.despawnAfter, 1.0f, 10.0f);
		script.projectileGravity = EditorGUILayout.Slider ("Gravity:", script.projectileGravity, 0.0f, 100.0f);
		script.projectileDrag = EditorGUILayout.Slider ("Drag:", script.projectileDrag, 0.0f, 10.0f);
		script.projectileAngularDrag = EditorGUILayout.Slider ("Angular Drag:", script.projectileAngularDrag, 0.0f, 10.0f);

		EditorGUILayout.Space();
	}
}
