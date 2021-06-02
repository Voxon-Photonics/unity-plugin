using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    public class VXProcess : Singleton<VXProcess> {
        
        #region constants
        public const string BuildDate = "2020/07/02";
        #endregion

        public static Runtime Runtime;

        #region inspector
        [FormerlySerializedAs("_guidelines")] [Tooltip("Will show capture volume of VX1 while emulating")]
        public bool guidelines;
        
        [Tooltip("Display volumes per second on front panel")]
        public bool show_vps = true;
        
        
        [Tooltip("Disable to turn hide version information on front panel")]
        public bool show_version = true;

        [Tooltip("Enable runtime applying VXGameobjects to all objects on load")]
        public bool apply_vx_on_load = true;

        [FormerlySerializedAs("_editor_camera")]
        [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
        [SerializeField]
        private VXCamera editorCamera;

        [Tooltip("Disable to turn off VXProcess behaviour")]
        public bool active = true;

        #endregion

        #region drawables
        public static List<IDrawable> Drawables = new List<IDrawable>(); 
        public static List<VXGameObject> Gameobjects = new List<VXGameObject>();
        #endregion

        #region internal_vars

        private string _dll_version = "";
        private VolumetricCamera _camera = new VolumetricCamera();
        static List<string> _logger = new List<string>();
        private List<float> frame_delta = new List<float>();
        #endregion

        #region public_vars

        public int _logger_max_lines = 10;

        #endregion
        #region getters_setters
        public VXCamera Camera
        {
            get => _camera?.Camera;
            set => _camera.Camera = value;
        }

        public Matrix4x4 Transform => _camera.Transform;

        public Vector3 EulerAngles
        {
            get => _camera.EulerAngles;
            set => _camera.EulerAngles = value;
        }

        public bool HasChanged => _camera.HasChanged;

        #endregion

        #region unity_functions
        private void Awake()
        {
            Drawables.Clear();
            Gameobjects.Clear();
        }

        private void Start () {

            Camera = editorCamera;
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

            if(Runtime == null)
            {
                Runtime = new Runtime();
            }
            // Load DLL
            if (!Runtime.isLoaded())
            {
                Runtime.Load();
                Runtime.Initialise();

                _dll_version = Runtime.GetDLLVersion().ToString().Substring(0, 8);
                
                #if (UNITY_EDITOR)
                    Debug.Log($"Voxon Unity Plugin. Compatible with Unity versions >= 2018.4");
                    Debug.Log($"Voxon Runtime version: {_dll_version}");
                    Debug.Log($"C# Interface version: {typeof(Voxon.IRuntimePromise).Assembly.GetName().Version}");
                #endif
            }
            else
            {
                Debug.LogWarning("DLL was already loaded!");
            }

            if(apply_vx_on_load){
                // Load all existing drawable components
                Renderer[] pack = FindObjectsOfType<Renderer>();
                foreach (Renderer piece in pack)
                {
                    if(piece.gameObject.GetComponent<ParticleSystem>())
                    {
                    }
                    else if(piece.gameObject.GetComponent<LineRenderer>() && !piece.gameObject.GetComponent<Line>())
                    {
                        piece.gameObject.AddComponent<Line>();
                    }
                    else
                    {
                        GameObject parent = piece.transform.root.gameObject;
                        if (!parent.GetComponent<VXGameObject>())
                        {
                            Gameobjects.Add(parent.AddComponent<VXGameObject>());
                        }
                    }
                
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!active || Runtime == null)
            {
                return;
            }

            bool isBreathing = Runtime.FrameStart();

            AudioListener.volume = Runtime.GetVolume();

            if (guidelines)
                Runtime.DrawGuidelines();

            if (show_version)
            {
                Runtime.LogToScreen(20, 560,$"DLL Version: {_dll_version}" );
            }
                

            // A camera must always be active while in process
            if (_camera != null && _camera.Camera == null)
            {
				Debug.LogError("No Active VXCamera!");
				active = false;
				Runtime.Shutdown();
				Runtime.Unload();
				Runtime.FrameEnd();
				return;
            }
            else if (_camera != null && _camera.HasChanged)
            {
                _camera?.ForceUpdate();
            }

            Draw();
            
            _camera?.ClearUpdated();

            // VX quit command; TODO this should be by choice
            if (Runtime.GetKey(0x1) || !isBreathing)
            {
                Runtime.Shutdown();
                Runtime.Unload();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
            }
        
            Runtime.FrameEnd();
        }

        private new void OnApplicationQuit()
        {
            if(Runtime != null) {
                Runtime.Shutdown();
                Runtime.Unload();
            }
        
            base.OnApplicationQuit();
        }

        #endregion

        #region drawing
        private void Draw()
        {
            foreach (IDrawable go in Drawables)
            {
                go.Draw();
            }

            while(_logger.Count > _logger_max_lines)
            {
                _logger.RemoveAt(0);
            }

            for(var idx = 0; idx < _logger.Count; idx++)
            {
                Runtime.LogToScreen(0, 64 + (idx * 8), _logger[idx]);
            }

            if (show_vps)
            {
                frame_delta.Add	(Time.deltaTime);
                while (frame_delta.Count > 60)
                {
                    frame_delta.RemoveAt(0);
                }

                float average_delta = 1 / frame_delta.Average();
                float max = (1/frame_delta.Min()); // Quickest Frame == highest vps
                float min = (1/frame_delta.Max());
                float max_dist = max - average_delta;
                float min_dist = min - average_delta;
                float dist = (Mathf.Abs(max) - Mathf.Abs(min)) / 2;
                Runtime.LogToScreen(20, 485, "VPS: " + (average_delta).ToString("F2") + " ( +/- " + dist.ToString("F2") + ")");
                Runtime.LogToScreen(40, 500, "Min Fps: " + min.ToString("F2"));
                Runtime.LogToScreen(40, 515, "Max Fps: " + max.ToString("F2"));
            }
        }
    
        public static void add_log_line(string str)
        {
            _logger.Add(str);
        }
        #endregion

        #region computing_transforms
        public static void ComputeTransform(ref Matrix4x4 targetWorld, ref Vector3[] vertices, ref point3d[] outPoltex)
        {
            if (vertices.Length != outPoltex.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(outPoltex));
            }

            if (Instance == null) return;

            // Build Camera transform
            Matrix4x4 matrix = Instance.Transform * targetWorld;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {

                var inV = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

                inV = matrix * inV;

                outPoltex[idx].x = inV.x;
                outPoltex[idx].y = -inV.z;
                outPoltex[idx].z = -inV.y;
            }
        }

        private static void ComputeTransform(ref Matrix4x4 targetWorld, ref Vector3[] vertices, ref Vector2[] uvs, ref poltex[] outPoltex)
        {
            if(vertices.Length != outPoltex.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(outPoltex));
            }

            // Build Camera transform
            Matrix4x4 matrix = Instance.Transform * targetWorld;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {
                var inV = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

                inV = matrix * inV;

                outPoltex[idx].x = inV.x;
                outPoltex[idx].y = -inV.z;
                outPoltex[idx].z = -inV.y;
                outPoltex[idx].u = uvs[idx].x;
                outPoltex[idx].v = uvs[idx].y;
            }
        }

        public static void ComputeTransform(ref Matrix4x4 target, ref Vector3[] vertices, ref poltex[] outPoltex)
        {
            var uvs = new Vector2[vertices.Length];

            ComputeTransform(ref target, ref vertices, ref uvs, ref outPoltex);
        }
        #endregion
    }
}
