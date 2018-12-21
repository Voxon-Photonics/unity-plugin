using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VXProcess : Singleton<VXProcess> {

    #region constants
    // Magic Numbers
    const int MAXCONTROLLERS = 4;
    #endregion

    #region inspector
    [Tooltip("Will show capture volume of VX1 while emulating")]
    public bool _guidelines = false;

    [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
    public GameObject _camera;

    [Tooltip("Disable to turn off VXProcess behaviour")]
    public bool active = true;
    #endregion


    // Drawable Items
    public static List<Voxon.IVXDrawable> _drawables = new List<Voxon.IVXDrawable>(); 
    public static List<Voxon.VXGameObject> _gameobjects = new List<Voxon.VXGameObject>();

    public static List<string> _logger = new List<string>();

    #region internal_vars
    Matrix4x4 _matrix_scale = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f));
    #endregion

    // Use this for initialization
    void Start () {
        // Should VX Load?
        if(!active)
        {
            return;
        }
        else if (_camera == null)
        {
            Debug.Log("No Camera Assigned. Disabling");
            active = false;
            return;
        }

        // Load DLL
        if (!Voxon.DLL.isLoaded())
        {
            Voxon.DLL.Load();
            Voxon.DLL.Initialise();
        }
        else
        {
            Debug.LogWarning("DLL was already loaded!");
        }

        // Load all existing drawable components
        var pack = FindObjectsOfType<Renderer>();
        foreach (Renderer piece in pack)
        {
            if(piece.gameObject.GetComponent<ParticleSystem>() && !piece.gameObject.GetComponent<Voxon.VXParticle>())
            {
                piece.gameObject.AddComponent<Voxon.VXParticle>();
            }
            else if(piece.gameObject.GetComponent<LineRenderer>() && !piece.gameObject.GetComponent<Voxon.VXLine>())
            {
                piece.gameObject.AddComponent<Voxon.VXLine>();
            }
            else
            {
                var parent = piece.transform.root.gameObject;
                if (!parent.GetComponent<Voxon.VXGameObject>())
                {
                    _gameobjects.Add(parent.AddComponent<Voxon.VXGameObject>());
                }
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!active)
        {
            return;
        }

        bool is_breathing = Voxon.DLL.start_frame();

        AudioListener.volume = Voxon.DLL.get_volume();

        if (_guidelines)
            Voxon.DLL.draw_guidelines();

        // A camera must always be active while in process
        if (_camera == null)
        {
            SetCamera(gameObject);
        }


        Draw();

        _camera.transform.hasChanged = false;

        // VX quit command; TODO this should be by choice
        if (Voxon.DLL.getkey(0x1) != 0 || !is_breathing)
        {
            Voxon.DLL.Shutdown();
            Voxon.DLL.Unload();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
        }
        
        Voxon.DLL.end_frame();
    }

    public void SetCamera(GameObject camera)
    {
        _camera = camera;
        _camera.transform.hasChanged = true;
        _matrix_scale = Matrix4x4.Scale(camera.transform.localScale);
    }

    private new void OnApplicationQuit()
    {
        Voxon.DLL.Shutdown();
        Voxon.DLL.Unload();
        base.OnApplicationQuit();
    }

    private void Draw()
    {
        foreach (var go in _drawables)
        {
            go.Draw();
        }

        while(_logger.Count > 10)
        {
            _logger.RemoveAt(0);
        }

        for(int idx = 0; idx < _logger.Count; idx++)
        {
            Voxon.DLL.debug_log(0, 64 + (idx * 8), _logger[idx]);
        }
    }

    public void add_log_line(string str)
    {
        _logger.Add(str);
    }

    public static void ComputeTransform(ref Matrix4x4 target_world, ref Vector3[] vertices, ref Vector2[] uvs, ref Voxon.DLL.poltex_t[] out_poltex)
    {
        if(vertices.Length != out_poltex.Length)
        {
            throw new System.ArgumentOutOfRangeException("Vertices size does not match out_poltex size");
        }

        // Build Camera transform
        Matrix4x4 Matrix = Instance._matrix_scale * Instance._camera.transform.worldToLocalMatrix * target_world;

        Vector4 in_v;
        for (int idx = vertices.Length - 1; idx >= 0; --idx)
        {
            in_v = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

            in_v = Matrix * in_v;

            out_poltex[idx].x = in_v.x;
            out_poltex[idx].y = -in_v.z;
            out_poltex[idx].z = -in_v.y;
            out_poltex[idx].u = vertices[idx].x;
            out_poltex[idx].v = vertices[idx].y;
        }
    }

    public static void ComputeTransform(ref Matrix4x4 target, ref Vector3[] vertices, ref Voxon.DLL.poltex_t[] out_poltex)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        ComputeTransform(ref target, ref vertices, ref uvs, ref out_poltex);
    }
}
