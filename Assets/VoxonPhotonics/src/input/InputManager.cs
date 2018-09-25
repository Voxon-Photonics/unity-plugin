using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
public class InputManager : EditorWindow
{
    static Vector2 scrollPosition;
    static InputManager _InputManager;

    [MenuItem("Voxon/Input Manager")]
    private static void Init()
    {
        _InputManager = (InputManager)GetWindow(typeof(InputManager));
        // Unneccesary but it shuts up Unity's warnings
        _InputManager.Show();
    }

    private void OnGUI()
    {
        
        Editor editor = Editor.CreateEditor(InputController.Instance);
        
        editor.OnInspectorGUI();
        
    }
}
#endif