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

	public static Voxon.Runtime runtime;

	#region inspector
	[Tooltip("Will show capture volume of VX1 while emulating")]
    public bool _guidelines = false;

    [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
    [SerializeField]
    private GameObject _editor_camera;

    [Tooltip("Disable to turn off VXProcess behaviour")]
    public bool active = true;
    #endregion

    #region drawables
    public static List<Voxon.IDrawable> _drawables = new List<Voxon.IDrawable>(); 
    public static List<Voxon.VXGameObject> _gameobjects = new List<Voxon.VXGameObject>();
    #endregion

    #region internal_vars
    Voxon.VolumetricCamera _camera = new Voxon.VolumetricCamera();
    static List<string> _logger = new List<string>();
    #endregion

    #region getters_setters
    public GameObject Camera
    {
        get { if (_camera != null) { return _camera.Camera; } else return null; }
        set { _camera.Camera = value; }
    }

    public Matrix4x4 Transform
    {
        get { return _camera.Transform; }
    }

    public Vector3 EulerAngles
    {
        get { return _camera.EulerAngles; }
        set { _camera.EulerAngles = value; }
    }

    public bool HasChanged
    {
        get { return _camera.HasChanged; }
    }
    #endregion

    #region unity_functions
    private void Awake()
    {
        _drawables.Clear();
        _gameobjects.Clear();
    }

    void Start () {

        Camera = _editor_camera;
        // Should VX Load?
        if (!active)
        {
            return;
        }
        else if (_camera.Camera == null)
        {
            Debug.Log("No Camera Assigned. Disabling");
            active = false;
            return;
        }

		if(runtime == null)
		{
			runtime = new Voxon.Runtime();
		}
        // Load DLL
        if (!runtime.isLoaded())
        {
			runtime.Load();
			runtime.Initialise();
        }
        else
        {
            Debug.LogWarning("DLL was already loaded!");
        }

        // Load all existing drawable components
        var pack = FindObjectsOfType<Renderer>();
        foreach (Renderer piece in pack)
        {
            if(piece.gameObject.GetComponent<ParticleSystem>())
            {
                continue;
            }
            else if(piece.gameObject.GetComponent<LineRenderer>() && !piece.gameObject.GetComponent<Voxon.Line>())
            {
                piece.gameObject.AddComponent<Voxon.Line>();
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
        if(!active && runtime != null)
        {
            return;
        }

        bool is_breathing = runtime.FrameStart();

        AudioListener.volume = runtime.GetVolume();

        if (_guidelines)
            runtime.DrawGuidelines();

        // A camera must always be active while in process
        if (_camera != null && _camera.Camera == null)
        {
            this.Camera = gameObject;
        }

        Draw();

        if(_camera != null) _camera.ClearUpdated();

        // VX quit command; TODO this should be by choice
        if (runtime.GetKey(0x1) || !is_breathing)
        {
            runtime.Shutdown();
            runtime.Unload();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
        }
        
        runtime.FrameEnd();
    }

    private new void OnApplicationQuit()
    {
		if(runtime != null) {
			runtime.Shutdown();
			runtime.Unload();
		}
        
        base.OnApplicationQuit();
    }

    #endregion

    #region drawing
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
            runtime.LogToScreen(0, 64 + (idx * 8), _logger[idx]);
        }
    }
    
    public void add_log_line(string str)
    {
        _logger.Add(str);
    }
    #endregion

    #region computing_transforms
    public static void ComputeTransform(ref Matrix4x4 target_world, ref Vector3[] vertices, ref Voxon.point3d[] out_poltex)
    {
        if (vertices.Length != out_poltex.Length)
        {
            throw new System.ArgumentOutOfRangeException("Vertices size does not match out_poltex size");
        }

        // Build Camera transform
        Matrix4x4 Matrix = Instance.Transform * target_world;

        Vector4 in_v;
        for (int idx = vertices.Length - 1; idx >= 0; --idx)
        {

            in_v = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

            in_v = Matrix * in_v;

            out_poltex[idx].x = in_v.x;
            out_poltex[idx].y = -in_v.z;
            out_poltex[idx].z = -in_v.y;
        }
    }

    public static void ComputeTransform(ref Matrix4x4 target_world, ref Vector3[] vertices, ref Vector2[] uvs, ref Voxon.poltex[] out_poltex)
    {
        if(vertices.Length != out_poltex.Length)
        {
            throw new System.ArgumentOutOfRangeException("Vertices size does not match out_poltex size");
        }

        // Build Camera transform
        Matrix4x4 Matrix = Instance.Transform * target_world;

        Vector4 in_v;
        for (int idx = vertices.Length - 1; idx >= 0; --idx)
        {
            in_v = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

            in_v = Matrix * in_v;

            out_poltex[idx].x = in_v.x;
            out_poltex[idx].y = -in_v.z;
            out_poltex[idx].z = -in_v.y;
            out_poltex[idx].u = uvs[idx].x;
            out_poltex[idx].v = uvs[idx].y;
        }
    }

    public static void ComputeTransform(ref Matrix4x4 target, ref Vector3[] vertices, ref Voxon.poltex[] out_poltex)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        ComputeTransform(ref target, ref vertices, ref uvs, ref out_poltex);
    }
	#endregion

	private void OnLevelWasLoaded(int level)
	{
		_drawables.Clear();
		_gameobjects.Clear();
	}
}
