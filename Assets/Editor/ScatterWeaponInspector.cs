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

[CustomEditor(typeof(ScatterBulletWeapon))]
public class ScatterWeaponInspector : Editor {

	public bool showSoundList = false;
	public bool showFireList = false;
	public bool showDmgTagsList = false;

	public override void OnInspectorGUI ()
	{
		ScatterBulletWeapon script = (ScatterBulletWeapon)target;
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
		tooltip = new GUIContent ("Hit Effect:", "Prefab Particle System to be played where the bullet hits");
		script.hitEffect = (ParticleSystem)EditorGUILayout.ObjectField (tooltip, script.hitEffect, typeof(ParticleSystem), true);
		tooltip = new GUIContent ("Hit Emission:", "Number of hit particles emitted when the bullet collides");
		script.hitParticles = (uint)EditorGUILayout.Slider (tooltip, script.hitParticles, 0, 100);
		tooltip = new GUIContent ("On Fire", "List of scripts to execute when the gun fires");
		showFireList = EditorGUILayout.Foldout(showFireList, tooltip);
		if (showFireList)
		{
			for (int i = 0; i < script.onFire.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("-", GUILayout.Width(23)))
					script.onFire.RemoveAt(i);
				else
					script.onFire[i] = (MBAction)EditorGUILayout.ObjectField ("", script.onFire[i], typeof(MBAction), true);

				EditorGUILayout.EndHorizontal();
			}
			if (script.onFire.Count > 0)
				EditorGUILayout.Space();
			if (GUILayout.Button("+", GUILayout.Width(23)))
			{
				script.onFire.Add(default(MBAction));
			}
		}

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

		EditorGUILayout.BeginHorizontal();
		tooltip = new GUIContent ("Fire Mode:", "Semi-Auto guns will fire once per mouse-click. Auto guns will fire continuously while the fire button is held down");
		EditorGUILayout.LabelField (tooltip);
		script.fMode = (FIRE_MODE)EditorGUILayout.EnumPopup (script.fMode);
		EditorGUILayout.EndHorizontal();
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
		tooltip = new GUIContent ("Damage Tags", "A list of tags which will either be ignored or exclusively damaged");
		showDmgTagsList = EditorGUILayout.Foldout(showDmgTagsList, tooltip);
		if (showDmgTagsList)
		{
			tooltip = new GUIContent ("Mode", "IgnoreSelected: Any objects with these tags will not be damaged.\nHitSelected: Any objects with these tags will be damaged. All others will be ignored");
			script.dmgTagsMode = (COLLISION_MODE)EditorGUILayout.EnumPopup (tooltip, script.dmgTagsMode);

			for (int i = 0; i < script.dmgTags.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("-", GUILayout.Width(23)))
					script.dmgTags.RemoveAt(i);
				else
					script.dmgTags[i] = EditorGUILayout.TextField(script.dmgTags[i]);

				EditorGUILayout.EndHorizontal();
			}
			if (script.dmgTags.Count > 0)
				EditorGUILayout.Space();
			if (GUILayout.Button("+", GUILayout.Width(23)))
			{
				script.dmgTags.Add("");
			}
		}

		tooltip = new GUIContent ("Shot Speed (rpm)", "Speed or the gun in Rounds Per Minute");
		script.speedRPM = EditorGUILayout.Slider (tooltip, script.speedRPM, 20.0f, 1500.0f);
		script.speed = script.speedRPM / 60;
		tooltip = new GUIContent ("Spread:", "Accuracy of shots, where 0 = completely accurate");
		script.spread = EditorGUILayout.Slider (tooltip, script.spread, 0.0f, 10.0f);
		EditorGUILayout.Space();

		/* BULLET WEAPON SETTINGS */
		EditorGUILayout.LabelField ("Scatter Weapon Specifics:", EditorStyles.boldLabel);

		tooltip = new GUIContent ("Projectile Count:", "Number of projectiles to be fired");
		script.projectileCount = (uint)EditorGUILayout.Slider (tooltip, script.projectileCount, 1, 20);
		tooltip = new GUIContent ("Hitscan Range:", "The maximum range of a bullet when using hitscan firing method");
		script.hitscanRange = EditorGUILayout.Slider (tooltip, script.hitscanRange, 5.0f, 200.0f);
		tooltip = new GUIContent ("Muzzle Velocity:", "While Hitscan method is in use, muzzle velocity affects the amount of force the projectile will exert on any rigidbody components collided with");
		script.bulletForce = EditorGUILayout.IntSlider (tooltip, script.bulletForce, 20, 150);

		EditorGUILayout.Space();

		/* HEAT MECHANICS */
		EditorGUILayout.LabelField ("Heat Mechanics:", EditorStyles.boldLabel);

		tooltip = new GUIContent ("Use Heat:", "Toggle the use of heat mechanics for this weapon");
		script.useHeatMechanics = EditorGUILayout.Toggle (tooltip, script.useHeatMechanics);
		if (script.useHeatMechanics)
		{
			// Show all heat mechanics settings here
			tooltip = new GUIContent ("Heat/Time:", "If true, 'heatRise' heat will be applied over one second while firing. \nElse 'heatRise' heat will be applied with each shot.");
			script.heatOverTime = EditorGUILayout.Toggle (tooltip, script.heatOverTime);
			tooltip = new GUIContent ("Heat Rise:", "Amount heat will rise while firing");
			script.heatRise = EditorGUILayout.Slider (tooltip, script.heatRise, 0, 100);

			tooltip = new GUIContent ("Instant Reset:", "If true, heat will instantly reset to 0 once cooldown begins. Else 'heatFall' heat will be subtracted over each second");
			script.instantHeatReset = EditorGUILayout.Toggle (tooltip, script.instantHeatReset);
			if (!script.instantHeatReset)
			{
				tooltip = new GUIContent ("   Heat Fall:", "Amount heat will fall while not firing");
				script.heatFall = EditorGUILayout.Slider (tooltip, script.heatFall, 0, 100);
			}

			tooltip = new GUIContent ("Cooldown Delay:", "Delay in seconds before the gun will begin to cool down");
			script.heatFallWait = EditorGUILayout.Slider (tooltip, script.heatFallWait, 0, 10);

			tooltip = new GUIContent ("Cool Enable:", "Firing will be re-enabled once the gun cools to this value");
			script.heatReEnable = EditorGUILayout.Slider (tooltip, script.heatReEnable, 0, 100);
		}

		EditorGUILayout.Space();
	}
}
