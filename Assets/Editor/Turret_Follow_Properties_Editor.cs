using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/* DESCRIPTION:
 * This scipt is a custom inspector which overrides the properties inspector for the
 * Turret_Follow script.
 */

[CustomEditor(typeof(Turret_FollowPlayer))]
public class Turret_Follow_Properties_Editor : Editor {

	public bool showTags = false;

	public override void OnInspectorGUI ()
	{
		Turret_FollowPlayer script = (Turret_FollowPlayer)target;

		// Show general information options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		script.rotationPiece = (Transform)EditorGUILayout.ObjectField ("Rotation Piece:", script.rotationPiece, typeof(Transform), true);
		script.muzzleEnd = (Transform)EditorGUILayout.ObjectField ("Shot Origin Point:", script.muzzleEnd, typeof(Transform), true);
		script.VisionAngle = EditorGUILayout.Slider ("Vision Angle:", script.VisionAngle, 40.0f, 90.0f);

		showTags = EditorGUILayout.Foldout(showTags, "See Through Tags");
		if (showTags)
		{
			for (int i = 0; i < script.SeeThroughTags.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("-", GUILayout.Width(23)))
					script.SeeThroughTags.RemoveAt(i);
				else
					script.SeeThroughTags[i] = EditorGUILayout.TextField(script.SeeThroughTags[i]);

				EditorGUILayout.EndHorizontal();
			}
			if (script.SeeThroughTags.Count > 0)
				EditorGUILayout.Space();
			if (GUILayout.Button("+", GUILayout.Width(23)))
			{
				script.SeeThroughTags.Add("");
			}
			EditorGUILayout.Space();


		}

		script.ShootDelayOnTargetAcquire = EditorGUILayout.Slider ("Shoot Delay:", script.ShootDelayOnTargetAcquire, 0.0f, 3.0f);

		// Show aim options
		EditorGUILayout.LabelField ("Aiming:", EditorStyles.boldLabel);
		script.TrackingRange = EditorGUILayout.Slider ("Tracking Range:", script.TrackingRange, 2.0f, 50.0f);
		script.MaxClampAngle = EditorGUILayout.Slider ("Up Aim Clamp:", script.MaxClampAngle, 0.0f, 88.0f);
		script.MinClampAngle = EditorGUILayout.Slider ("Down Aim Clamp:", script.MinClampAngle, 0.0f, 88.0f);
		script.rotateSpeed = EditorGUILayout.Slider ("Rotation Speed:", script.rotateSpeed, 1.0f, 10.0f);
	}
}
