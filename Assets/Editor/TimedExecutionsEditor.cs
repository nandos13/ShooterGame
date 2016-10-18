using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/* DESCRIPTION:
 * This scipt is a custom inspector which overrides the properties inspector for the
 * TimedExecutions script. This allows the inspector to show the list of ActionValue class
 * instances, as they are custom classes with more than one editable variable.
 */

[CustomEditor(typeof(TimedExecutions))]
public class TimedExecusionsEditor : Editor {

	public override void OnInspectorGUI ()
	{
		TimedExecutions script = (TimedExecutions)target;

		for (int i = 0; i < script.actions.Count; i++)
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
		
			if (GUILayout.Button("-", GUILayout.Width(23)))
				script.actions.RemoveAt(i);
			else
			{
				script.actions[i].action = (MBAction)EditorGUILayout.ObjectField ("", script.actions[i].action, typeof(MBAction), true);
				EditorGUILayout.EndHorizontal();

				script.actions[i].value = EditorGUILayout.Slider ("      Time (sec): ", script.actions[i].value, 0, 600);
			}
		}
		if (script.actions.Count > 0)
			EditorGUILayout.Space();
		if (GUILayout.Button("+", GUILayout.Width(23)))
		{
			script.actions.Add(new ActionValue());
		}
		EditorGUILayout.Space();
	}
}
