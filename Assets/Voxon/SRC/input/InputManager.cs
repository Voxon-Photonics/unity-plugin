using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Voxon
{
    public class InputManager : EditorWindow
    {
        private static Vector2 _scrollPosition;
        private static InputManager _inputManager;

        [MenuItem("Voxon/Input Manager")]
        private static void Init()
        {
            _inputManager = (InputManager)GetWindow(typeof(InputManager));
            // Annoying but it shuts up Unity's warnings
            _inputManager.Show();
        }

        private void OnGUI()
        {
        
            Editor editor = Editor.CreateEditor(InputController.Instance);
        
            editor.OnInspectorGUI();
        
        }
    }
}
#endif