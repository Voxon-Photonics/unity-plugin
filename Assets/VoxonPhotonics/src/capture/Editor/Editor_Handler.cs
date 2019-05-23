using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoadAttribute]
public class Editor_Handler: MonoBehaviour {

    static Editor_Handler()
    {
        EditorApplication.playModeStateChanged += PlayStateChange;
        try
        {
            if (FindObjectOfType<VXProcess>() == null)
            {
                // Force creation of VXProcess
                VXProcess A = VXProcess.Instance;
            }

            if(AssetDatabase.IsValidFolder("Assets/StreamingAssets") == false)
            {
                System.IO.Directory.CreateDirectory("Assets\\StreamingAssets");
            }

            if(InputController.GetKey("Quit") == 0)
            {
                InputController.LoadData();
            }

            Debug.Assert(InputController.GetKey("Quit") != 0, "No 'Quit' keybinding found. Add to Input Manager");
        }
        catch(System.InvalidOperationException E)
        {
            Debug.Log(E.Message);
        }

        DefaultPlayerSettings();
    }

    [MenuItem("Voxon/Tools/Reimport Textures")]
    static void reimportMaterials()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2d", null);
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            texImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        EditorUtility.DisplayDialog("Reimported Textures", "Textures Reimported.", "Ok");
    }

    [MenuItem("Voxon/Tools/Reimport Meshes")]
    static void reimportMeshes()
    {
        var guids = AssetDatabase.FindAssets("t:Mesh", null);
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter meshImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            meshImporter.isReadable = true;
            meshImporter.useFileScale = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        EditorUtility.DisplayDialog("Reimported Textures", "Textures Reimported.", "Ok");
    }

    [MenuItem("Voxon/Tools/Prebuild Mesh")]
    public static void prebuildMesh()
    {
        var guids = AssetDatabase.FindAssets("t:Mesh", null);

        MeshRegister meshRegister = new MeshRegister();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path == "") continue;

            Mesh t = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
            meshRegister.get_registed_mesh(ref t);
        }

        // Create an instance of the type and serialize it.
        IFormatter formatter = new BinaryFormatter();
        FileStream s = null;
        
        try
        {
            if (!AssetDatabase.IsValidFolder(Application.dataPath + "/StreamingAssets"))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/StreamingAssets");
            }

            s = new FileStream(Application.dataPath + "/StreamingAssets/MeshData.bin", FileMode.Create);
            formatter.Serialize(s, meshRegister.PackMeshes());
        }
        finally
        {
            s.Close();
        }

        EditorUtility.DisplayDialog("Prebuild Mesh", (meshRegister.Length().ToString() + " Meshes Processed"), "Ok");
    }

    private static void PlayStateChange(PlayModeStateChange state)
    {
        // Handle Editor play states (block Play when Input disabled / close VX when Play stopped)
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Editor Play Stopping : Shutting down VX1 Simulator");
			VXProcess.runtime.Shutdown();
        }
    }

    public static void DefaultPlayerSettings()
    {
        PlayerSettings.allowFullscreenSwitch = false;
        PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        PlayerSettings.defaultScreenHeight = 480;
        PlayerSettings.defaultScreenWidth = 640;
        // PlayerSettings.displayResolutionDialog = UnityEditor.ResolutionDialogSetting.Disabled;
        PlayerSettings.forceSingleInstance = true;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.runInBackground = true;
        PlayerSettings.usePlayerLog = true;
        PlayerSettings.visibleInBackground = true;
    }
}
