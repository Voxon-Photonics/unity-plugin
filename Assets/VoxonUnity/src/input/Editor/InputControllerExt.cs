using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputController))]
public class InputControllerExt : Editor {
    public static Vector2 scrollPosition = new Vector2();
    public override void OnInspectorGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height-20));

        base.OnInspectorGUI();
        
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New"))
        {
            InputController.Instance.Keyboard.Clear();
            InputController.Instance.Mouse.Clear();
            InputController.Instance.J1Axis.Clear();
            InputController.Instance.J1Buttons.Clear();
            InputController.Instance.J2Axis.Clear();
            InputController.Instance.J2Buttons.Clear();
            InputController.Instance.J3Axis.Clear();
            InputController.Instance.J3Buttons.Clear();
            InputController.Instance.J4Axis.Clear();
            InputController.Instance.J4Buttons.Clear();
        }

        if (GUILayout.Button("Save"))
        {
            InputController.SaveData();
        }
        if (GUILayout.Button("Load"))
        {
            InputController.LoadData();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }
}
