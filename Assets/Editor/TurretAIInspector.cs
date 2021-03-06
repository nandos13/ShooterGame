﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/* DESCRIPTION:
 * This scipt is a custom inspector which overrides the properties inspector for the
 * TurretAI script. This allows the inspector to hide or show particular variables
 * based on previously selected variables. Also allows a custom layout and an
 * over all nicer looking inspector.
 */

[CustomEditor(typeof(TurretAI))]
public class TurretAIInspector : Editor {

	public bool showTags = false;
	public bool showGunList = false;

	public override void OnInspectorGUI ()
	{
		TurretAI script = (TurretAI)target;
		GUIContent tooltip = new GUIContent ("", "");

		// Show general information options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Rotation Piece:", "The part of the turret that will rotate to look towards the target");
		script.rotationPiece = (Transform)EditorGUILayout.ObjectField (tooltip, script.rotationPiece, typeof(Transform), true);
		tooltip = new GUIContent ("Search Mode:", "Will the turret search randomly using the old search method, or scan point to point in a sweeping motion?");
		script.searchMode = (TURRET_SEARCH_TYPE)EditorGUILayout.EnumPopup (tooltip, script.searchMode);
		if (script.searchMode == TURRET_SEARCH_TYPE.PointToPoint)
		{
			tooltip = new GUIContent ("   Search Angle:", "The angle the turret will sweep to, where 0 is straight ahead");
			script.searchAngle = EditorGUILayout.Slider (tooltip, script.searchAngle, 0.0f, 90.0f);
			tooltip = new GUIContent ("   Rotation Pause:", "Time in seconds the turret will stop rotating for at the end of each directional sweep");
			script.pauseTime = EditorGUILayout.Slider (tooltip, script.pauseTime, 0.0f, 1.5f);
		}
		tooltip = new GUIContent ("Vision Angle:", "Angle in degrees the turret can see the target");
		script.visionAngle = EditorGUILayout.Slider (tooltip, script.visionAngle, 40.0f, 90.0f);

		tooltip = new GUIContent ("Invisible Tags", "A list of tags the turret can see through");
		showTags = EditorGUILayout.Foldout(showTags, tooltip);
		if (showTags)
		{
			for (int i = 0; i < script.seeThroughTags.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("-", GUILayout.Width(23)))
					script.seeThroughTags.RemoveAt(i);
				else
					script.seeThroughTags[i] = EditorGUILayout.TextField(script.seeThroughTags[i]);

				EditorGUILayout.EndHorizontal();
			}
			if (script.seeThroughTags.Count > 0)
				EditorGUILayout.Space();
			if (GUILayout.Button("+", GUILayout.Width(23)))
			{
				script.seeThroughTags.Add("");
			}
			EditorGUILayout.Space();
		}

		// Show gun information options
		EditorGUILayout.LabelField ("Guns:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Auto Populate List:", "If checked, the script will automatically find and use all guns attached to the turret. Otherwise you can specify specific gun scripts to be used");
		script.autoPopulateGunList = EditorGUILayout.Toggle(tooltip, script.autoPopulateGunList);

		if (!script.autoPopulateGunList)
		{
			// Show gun list so it can be manually modified
			tooltip = new GUIContent ("Guns", "A list of attached guns the turret will use");
			showGunList = EditorGUILayout.Foldout(showGunList, tooltip);
			if (showGunList)
			{
				for (int i = 0; i < script.guns.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button("-", GUILayout.Width(23)))
						script.guns.RemoveAt(i);
					else
						script.guns[i] = (WeaponBase)EditorGUILayout.ObjectField ("", script.guns[i], typeof(WeaponBase), true);

					EditorGUILayout.EndHorizontal();
				}
				if (script.seeThroughTags.Count > 0)
					EditorGUILayout.Space();
				if (GUILayout.Button("+", GUILayout.Width(23)))
				{
					script.guns.Add(default(WeaponBase));
				}
				EditorGUILayout.Space();
			}
		}

		tooltip = new GUIContent ("Shoot Delay:", "Delay in seconds before the turret will shoot, after obtaining a direct view to it's target");
		script.shootDelay = EditorGUILayout.Slider (tooltip, script.shootDelay, 0.0f, 3.0f);

		// Show aim options
		EditorGUILayout.LabelField ("Aiming:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Tracking Range:", "View distance of the turret");
		script.trackingRange = EditorGUILayout.Slider (tooltip, script.trackingRange, 10.0f, 500.0f);
		tooltip = new GUIContent ("Aim-Up Clamp:", "Max angle the turret can look up");
		script.clampAngleUp = EditorGUILayout.Slider (tooltip, script.clampAngleUp, 0.0f, 88.0f);
		tooltip = new GUIContent ("Aim-Down Clamp:", "Max angle the turret can look down");
		script.clampAngleDown = EditorGUILayout.Slider (tooltip, script.clampAngleDown, 0.0f, 88.0f);
		tooltip = new GUIContent ("Rotation Speed:", "The speed the turret will rotate at");
		script.rotateSpeed = EditorGUILayout.Slider (tooltip, script.rotateSpeed, 1.0f, 10.0f);
	}
}
