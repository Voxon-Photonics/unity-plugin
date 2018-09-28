using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VXProcess : Singleton<VXProcess> {

    [Tooltip("Will show capture volume of VX1 while emulating")]
    public bool _guidelines = false;

    [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
    public GameObject _camera;

    [Tooltip("Disable to turn off VXProcess behaviour")]
    public bool active = true;

    // Magic Numbers
    const int MAXCONTROLLERS = 4;

    // Drawable Items
    public static List<Voxon.IVXDrawable> _drawables = new List<Voxon.IVXDrawable>(); 
    public static List<Voxon.VXGameObject> _gameobjects = new List<Voxon.VXGameObject>();

    public static List<string> _logger = new List<string>();

    
    // Use this for initialization
    void Start () {
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

        if (!Voxon.DLL.isLoaded())
        {
            Voxon.DLL.Load();
            Voxon.DLL.Initialise();
        }
        else
        {
            Debug.LogWarning("DLL was already loaded!");
        }


        var pack = FindObjectsOfType<Renderer>();
        foreach (Renderer piece in pack)
        {
            if(piece.gameObject.GetComponent<ParticleSystem>())
            {
                piece.gameObject.AddComponent<Voxon.VXParticle>();
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

        if(_guidelines)
            Voxon.DLL.draw_guidelines();

        if(_camera == null)
        {
            SetCamera(gameObject);
        }

        Draw();

        _camera.transform.hasChanged = false;

        if(Voxon.DLL.getkey(0x1) != 0 || !is_breathing)
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
}
