using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/* DESCRIPTION:
 * This script is a cunstom inspector which overrides the properties inspector for the
 * BulletWeapon script. This allows the inspector to hide or show particular variables
 * based on previously selected variables. Also allows a custom layout and an
 * over all nicer looking inspector.
 */

[CustomEditor(typeof(BulletWeapon))]
public class BulletWeaponInspector : Editor {

	public bool showSoundList = false;

	public override void OnInspectorGUI ()
	{
		BulletWeapon script = (BulletWeapon)target;
		GUIContent tooltip = new GUIContent ("", "");

		/* GENERAL SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Origin:", "Transform at the end of the muzzle where projectiles originate from");
		script.shotOrigin = (Transform)EditorGUILayout.ObjectField (tooltip, script.shotOrigin, typeof(Transform), true);
		tooltip = new GUIContent ("Audio Source:", "An audio source to handle gun sounds");
		script.audioSrc = (AudioSource)EditorGUILayout.ObjectField (tooltip, script.audioSrc, typeof(AudioSource), true);

		/* VISUAL SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Visual:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Muzzle Flash:", "Particle System at the end of the muzzle. Played when the gun is fired");
		script.muzzleFlash = (ParticleSystem)EditorGUILayout.ObjectField (tooltip, script.muzzleFlash, typeof(ParticleSystem), true);
		tooltip = new GUIContent ("Muzzle Emission:", "Number of muzzle particles emitted when the gun is shot");
		script.muzzleParticles = (uint)EditorGUILayout.Slider (tooltip, script.muzzleParticles, 0, 100);
		tooltip = new GUIContent ("Hit Effect:", "Prefab Particle System to be played where the bullet hits");
		script.hitEffect = (ParticleSystem)EditorGUILayout.ObjectField (tooltip, script.hitEffect, typeof(ParticleSystem), true);
		tooltip = new GUIContent ("Hit Emission:", "Number of hit particles emitted when the bullet collides");
		script.hitParticles = (uint)EditorGUILayout.Slider (tooltip, script.hitParticles, 0, 100);

		/* AUDIO SETTINGS */
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Audio:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Fire sounds", "A list of sounds that can be played when the gun is fired. One will be picked at random");
		showSoundList = EditorGUILayout.Foldout(showSoundList, tooltip);
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
		tooltip = new GUIContent ("Bottomless Clip:", "Toggle: Will the currently loaded clip ever run out of ammo or fire forever?");
		script.bottomlessClip = EditorGUILayout.Toggle (tooltip, script.bottomlessClip);
		// Clip size and maximum ammo variables only show if the weapon is not bottomless clip
		if (!script.bottomlessClip) 
		{
			tooltip = new GUIContent ("   Clip Size:", "Size of a single clip");
			script.clipSize = (uint)EditorGUILayout.Slider (tooltip, script.clipSize, 1, 300);
			tooltip = new GUIContent ("   Unlimited Ammo:", "Toggle: Will a new clip be used when the gun is reloaded?");
			script.unlimitedAmmo = EditorGUILayout.Toggle (tooltip, script.unlimitedAmmo);

			// Starting ammo variable is only shown if the weapon does not have unlimited ammo
			if (!script.unlimitedAmmo) 
			{
				tooltip = new GUIContent ("      Starting Ammo:", "The amount of ammo the gun will start with. This includes currently loaded ammo");
				script.startingAmmo = (uint)EditorGUILayout.Slider (tooltip, script.startingAmmo, 1, 500);
			}
		}
		tooltip = new GUIContent ("Damage:", "The amount of damage a single bullet will inflict on collision");
		script.damage = EditorGUILayout.Slider (tooltip, script.damage, 0.1f, 300.0f);
		tooltip = new GUIContent ("Shot Speed (rpm)", "Speed or the gun in Rounds Per Minute");
		script.speedRPM = EditorGUILayout.Slider (tooltip, script.speedRPM, 20.0f, 1500.0f);
		script.speed = script.speedRPM / 60;
		tooltip = new GUIContent ("Spread:", "Accuracy of shots, where 0 = completely accurate");
		script.spread = EditorGUILayout.Slider (tooltip, script.spread, 0.0f, 10.0f);
		EditorGUILayout.Space();

		/* BULLET WEAPON SETTINGS */
		EditorGUILayout.LabelField ("Bullet Weapon Specifics:", EditorStyles.boldLabel);

		tooltip = new GUIContent ("Use Hitscan:", "Toggle: Will the gun use the hitscan method (using instant raycasting), or fire physical bullet projectiles?");
		script.hitscan = EditorGUILayout.Toggle (tooltip, script.hitscan);
		if (!script.hitscan)
		{
			// Show physical bullet options
			tooltip = new GUIContent ("Projectile:", "Prefab GameObject to be fired out of the gun");
			GameObject proj = (GameObject)EditorGUILayout.ObjectField (tooltip, script.bulletProjectile, typeof(Object), false);
			// Do not allow a prefab without a rigidbody
			if (proj)
			{
				Rigidbody rb = proj.GetComponent<Rigidbody>();
				if (!rb)
				{
					proj = null;
					Debug.LogError("Error: You tried to add a bullet Projectile without a rigidbody component! Please ensure the prefab has a rigidbody attached.");
				}
				else
					script.bulletProjectile = proj;
			}
			tooltip = new GUIContent ("Muzzle Velocity:", "Initial velocity of the projectile when being fired");
			script.bulletForce = EditorGUILayout.IntSlider (tooltip, script.bulletForce, 20, 150);
			tooltip = new GUIContent ("Despawn Time:", "Time in seconds the projectile will stay alive for before despawning");
			script.despawnAfter = EditorGUILayout.Slider ("Despawn Time:", script.despawnAfter, 1.0f, 10.0f);
		}
		else
		{
			tooltip = new GUIContent ("Muzzle Velocity:", "While Hitscan method is in use, muzzle velocity affects the amount of force the projectile will exert on any rigidbody components collided with");
			script.bulletForce = EditorGUILayout.IntSlider ("Muzzle Velocity:", script.bulletForce, 20, 150);
		}

		EditorGUILayout.Space();
	}
}
