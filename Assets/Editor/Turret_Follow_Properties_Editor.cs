using UnityEngine;
using System.Collections;
using UnityEditor;

/* DESCRIPTION:
 */

[CustomEditor(typeof(Turret_FollowPlayer))]
public class Turret_Follow_Properties_Editor : Editor {

	public override void OnInspectorGUI ()
	{
		Turret_FollowPlayer script = (Turret_FollowPlayer)target;

		// Show general information options
		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("General:", EditorStyles.boldLabel);
		script.muzzleEnd = (Transform)EditorGUILayout.ObjectField ("Shot Origin Point:", script.muzzleEnd, typeof(Transform), true);
		script.visionPoint = (Transform)EditorGUILayout.ObjectField ("Vision Point:", script.visionPoint, typeof(Transform), true);
		script.VisionAngle = EditorGUILayout.Slider ("Vision Angle:", script.VisionAngle, 20.0f, 90.0f);
		script.ShootDelayOnTargetAcquire = EditorGUILayout.Slider ("Shoot Delay:", script.ShootDelayOnTargetAcquire, 0.0f, 3.0f);

		// Show aim options
		EditorGUILayout.LabelField ("Aim:", EditorStyles.boldLabel);
		script.TrackingRange = EditorGUILayout.Slider ("Tracking Range:", script.TrackingRange, 2.0f, 50.0f);
		script.MaxClampAngle = EditorGUILayout.Slider ("Up Aim Clamp:", script.MaxClampAngle, 0.0f, 88.0f);
		script.MinClampAngle = EditorGUILayout.Slider ("Down Aim Clamp:", script.MinClampAngle, 0.0f, 88.0f);
		script.rotateSpeed = EditorGUILayout.Slider ("Rotation Speed:", script.rotateSpeed, 1.0f, 10.0f);
	}
}
