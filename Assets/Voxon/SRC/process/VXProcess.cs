using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Voxon
{
    public class VXProcess : Singleton<VXProcess> {

   
        #region constants
        #endregion


        public static Runtime Runtime;

        #region inspector
        [FormerlySerializedAs("_guidelines")] [Tooltip("Will show capture volume of VX1 while emulating")]
        public bool guidelines;

        [FormerlySerializedAs("_editor_camera")]
        [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
        [SerializeField]
        private GameObject editorCamera;

        [Tooltip("Disable to turn off VXProcess behaviour")]
        public bool active = true;
        
        [Tooltip("Error texture")]
        public Texture2D ErrorTexture;
        #endregion

        #region drawables
        public static List<IDrawable> Drawables = new List<IDrawable>(); 
        public static List<VXGameObject> Gameobjects = new List<VXGameObject>();
        #endregion

        #region internal_vars

        private VolumetricCamera _camera = new VolumetricCamera();
        static List<string> _logger = new List<string>();
        #endregion

        #region getters_setters
        public GameObject Camera
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
            }
            else
            {
                Debug.LogWarning("DLL was already loaded!");
            }

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

        // Update is called once per frame
        void Update()
        {
            if(!active && Runtime != null)
            {
                return;
            }

            bool isBreathing = Runtime.FrameStart();

            AudioListener.volume = Runtime.GetVolume();

            if (guidelines)
                Runtime.DrawGuidelines();

            // A camera must always be active while in process
            if (_camera != null && _camera.Camera == null)
            {
                Camera = gameObject;
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

            while(_logger.Count > 10)
            {
                _logger.RemoveAt(0);
            }

            for(var idx = 0; idx < _logger.Count; idx++)
            {
                Runtime.LogToScreen(0, 64 + (idx * 8), _logger[idx]);
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
