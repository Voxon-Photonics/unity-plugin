using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
public class ProcessManager : EditorWindow
{
    static ProcessManager _ProcessManager;

    [MenuItem("Voxon/Process")]
    private static void Init()
    {
        _ProcessManager = (ProcessManager)GetWindow(typeof(ProcessManager));
    }

    private void OnGUI()
    {
        Editor editor = Editor.CreateEditor(VXProcess.Instance);
        editor.OnInspectorGUI();
    }
}
#endif