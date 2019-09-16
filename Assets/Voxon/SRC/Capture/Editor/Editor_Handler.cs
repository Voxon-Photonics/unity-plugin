using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Voxon
{
    [InitializeOnLoad]
    public class EditorHandler: MonoBehaviour {

        static EditorHandler()
        {
            #if !UNITY_2018_4
                Debug.LogWarning("Voxon Plugin may not be compatible with this version of Unity. Please use 2018.4.X (LTS)");
            #endif
            
            EditorApplication.playModeStateChanged += PlayStateChange;
            try
            {
                if (FindObjectOfType<VXProcess>() == null)
                {
                    // Force creation of VXProcess
                    VXProcess a = VXProcess.Instance;
                }

                if(AssetDatabase.IsValidFolder("Assets/StreamingAssets") == false)
                {
                    Directory.CreateDirectory("Assets\\StreamingAssets");
                }

                if(InputController.GetKey("Quit") == 0)
                {
                    InputController.LoadData();
                }

                Debug.Assert(InputController.GetKey("Quit") != 0, "No 'Quit' keybinding found. Add to Input Manager");
            }
            catch(System.InvalidOperationException e)
            {
                Debug.Log(e.Message);
            }

            DefaultPlayerSettings();
        }

        [MenuItem("Voxon/Tools/Reimport Textures")]
        static void ReimportMaterials()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2d", null);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (texImporter != null) texImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Texture2D tex = AssetDatabase.LoadAssetAtPath (path, typeof(Texture2D)) as Texture2D;
                if (tex.height != tex.width)
                {
                    EditorUtility.DisplayDialog("Reimported Textures Error", path + " texture not uniform.", "Ok");
                    Debug.LogError(path + " texture not uniform. Will crash on play");
                }
            }
            EditorUtility.DisplayDialog("Reimported Textures", "Textures Reimported.", "Ok");
        }

        [MenuItem("Voxon/Tools/Reimport Meshes")]
        static void ReimportMeshes()
        {
            string[] guids = AssetDatabase.FindAssets("t:Mesh", null);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var meshImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (meshImporter != null)
                {
                    meshImporter.isReadable = true;
                }

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            EditorUtility.DisplayDialog("Reimported Meshes", "Meshes Reimported.", "Ok");
        }

        [MenuItem("Voxon/Tools/Prebuild Mesh")]
        public static void PrebuildMesh()
        {
            string[] guids = AssetDatabase.FindAssets("t:Mesh", null);

            MeshRegister meshRegister = MeshRegister.Instance;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == "") continue;

                var t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                meshRegister.get_registed_mesh(ref t);
            }

            // Create an instance of the type and serialize it.
            IFormatter formatter = new BinaryFormatter();

            if (!AssetDatabase.IsValidFolder($"{Application.dataPath}/StreamingAssets"))
            {
                Directory.CreateDirectory($"{Application.dataPath}/StreamingAssets");
            }
        
            using (var s = new FileStream($"{Application.dataPath}/StreamingAssets/MeshData.bin", FileMode.Create))
            {
                formatter.Serialize(s, meshRegister.PackMeshes());
            }

            EditorUtility.DisplayDialog("Prebuild Mesh", message: ($"{meshRegister.Length()} Meshes Processed"),
                "Ok");
        }

        private static void PlayStateChange(PlayModeStateChange state)
        {
            // Handle Editor play states (block Play when Input disabled / close VX when Play stopped)
            if (state != PlayModeStateChange.ExitingPlayMode || VXProcess.Runtime == null) return;
        
            Debug.Log("Editor Play Stopping : Shutting down VX1 Simulator");
            VXProcess.Runtime.Shutdown();
        }

        private static void DefaultPlayerSettings()
        {
#if UNITY_2017
		PlayerSettings.displayResolutionDialog = UnityEditor.ResolutionDialogSetting.Disabled;
		PlayerSettings.defaultIsFullScreen = true;
#else
            PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
#endif

            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
            PlayerSettings.allowFullscreenSwitch = false;        
            PlayerSettings.defaultScreenHeight = 480;
            PlayerSettings.defaultScreenWidth = 640;
            PlayerSettings.forceSingleInstance = true;
            PlayerSettings.resizableWindow = false;
            PlayerSettings.runInBackground = true;
            PlayerSettings.usePlayerLog = true;
            PlayerSettings.visibleInBackground = true;
        }
    }
}
