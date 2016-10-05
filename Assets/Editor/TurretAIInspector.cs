using UnityEngine;
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

	public override void OnInspectorGUI ()
	{
		TurretAI script = (TurretAI)target;
		GUIContent tooltip = new GUIContent ("", "");

		// Show general information options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Rotation Piece:", "The part of the turret that will rotate to look towards the target");
		script.rotationPiece = (Transform)EditorGUILayout.ObjectField (tooltip, script.rotationPiece, typeof(Transform), true);
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

		tooltip = new GUIContent ("Shoot Delay:", "Delay in seconds before starting to fire");
		script.shootDelay = EditorGUILayout.Slider (tooltip, script.shootDelay, 0.0f, 3.0f);

		// Show aim options
		EditorGUILayout.LabelField ("Aiming:", EditorStyles.boldLabel);
		tooltip = new GUIContent ("Tracking Range:", "View distance of the turret");
		script.trackingRange = EditorGUILayout.Slider (tooltip, script.trackingRange, 2.0f, 50.0f);
		tooltip = new GUIContent ("Aim-Up Clamp:", "Max angle the turret can look up");
		script.clampAngleUp = EditorGUILayout.Slider (tooltip, script.clampAngleUp, 0.0f, 88.0f);
		tooltip = new GUIContent ("Aim-Down Clamp:", "Max angle the turret can look down");
		script.clampAngleDown = EditorGUILayout.Slider (tooltip, script.clampAngleDown, 0.0f, 88.0f);
		tooltip = new GUIContent ("Rotation Speed:", "The speed the turret will rotate at");
		script.rotateSpeed = EditorGUILayout.Slider (tooltip, script.rotateSpeed, 1.0f, 10.0f);
	}
}
