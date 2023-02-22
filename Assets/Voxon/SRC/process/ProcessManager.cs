﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Voxon
{
    public class ProcessManager : EditorWindow
    {
        private static ProcessManager _processManager;

        [MenuItem("Voxon/Process")]
        private static void Init()
        {
            _processManager = (ProcessManager)GetWindow(typeof(ProcessManager));
            // Unnecessary but it shuts up Unity's warnings


            _processManager.titleContent = new UnityEngine.GUIContent("Voxon Unity Plugin");




            // Limit size of the window
            _processManager.minSize = new Vector2(450, 470);
            _processManager.maxSize = new Vector2(1920, 470);

            _processManager.Show();
        }

        private void OnGUI()
        {
            Editor editor = Editor.CreateEditor(VXProcess.Instance);
            editor.OnInspectorGUI();
        }
    }
}
#endif