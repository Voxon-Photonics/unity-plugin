using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(VXCamera))]
[CanEditMultipleObjects]
public class VXCameraEditor : Editor
{
	SerializedProperty uniformScale;
	SerializedProperty baseScale;
	SerializedProperty vectorScale;

	SerializedProperty loadViewFinder;
	SerializedProperty ViewFinderDimensions;

	bool showScalars = true;
	bool showViewFinder = true;

	void OnEnable()
	{
		uniformScale = serializedObject.FindProperty("uniformScale");
		baseScale = serializedObject.FindProperty("baseScale");
		vectorScale = serializedObject.FindProperty("vectorScale");

		loadViewFinder = serializedObject.FindProperty("loadViewFinder");
		ViewFinderDimensions = serializedObject.FindProperty("ViewFinderDimensions");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

#if UNITY_2019_4_OR_NEWER
		showScalars = EditorGUILayout.BeginFoldoutHeaderGroup(showScalars, "Camera Scale");
#endif
		if (showScalars) { 
			EditorGUILayout.PropertyField(uniformScale);
			if (uniformScale.boolValue)
			{
				EditorGUILayout.PropertyField(baseScale);
			} else
			{
				EditorGUILayout.PropertyField(vectorScale);
			}
		}
#if UNITY_2019_4_OR_NEWER
		EditorGUILayout.EndFoldoutHeaderGroup();
#endif

#if UNITY_2019_4_OR_NEWER
		showViewFinder = EditorGUILayout.BeginFoldoutHeaderGroup(showViewFinder, "View Finder");
#endif
		if (showViewFinder) { 
			EditorGUILayout.PropertyField(loadViewFinder);
			if (!loadViewFinder.boolValue)
			{
				EditorGUILayout.PropertyField(ViewFinderDimensions);
			}
		}
#if UNITY_2019_4_OR_NEWER
		EditorGUILayout.EndFoldoutHeaderGroup();
#endif
		serializedObject.ApplyModifiedProperties();
	}
}