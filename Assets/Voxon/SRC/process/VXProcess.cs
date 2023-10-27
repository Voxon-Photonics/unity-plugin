using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    public class VXProcess : Singleton<VXProcess> {

		#region types
		public enum RecordingType { SINGLE_FRAME, ANIMATION };
        #endregion

        #region timer
        double startTime = 0;
        double startDelay = 0;
        double awakeDelay = 0;
        const double EDITOR_AWAKE_DELAY = 1.5;
        #endregion


        #region constants
        public const string BuildDate = "20230522";
        #endregion

        public static Runtime Runtime;

        #region inspector

        [Header("Editor")]
        [Tooltip("Delaying the Voxon Startup in editor makes the Voxon plugin more stable")]
        private bool delayStartUpInEditor = true;

        [Tooltip("Force Restart of VX Runtime")]
        private bool restartVXRuntime = false;
		
        [Tooltip("Instally kills the VX Runtime (if its lingering in memory)")]
        public bool killVXRuntime = false;

        [Tooltip("hit a breakpoint in the VxProcess Script (requires VS to be in debug mode)")]
        public bool debugVxProcess = false;


        [Header("Debug")]
        [FormerlySerializedAs("_guidelines")] [Tooltip("Will show capture volume of VX1 while emulating")]
        public bool guidelines;
        
        [Tooltip("Display volumes per second on front panel")]
        public bool show_vps = true;
        
        
        [Tooltip("Disable to turn hide version information on front panel")]
        public bool show_version = true;

		[Header("Camera")]
		[Tooltip("Disable to turn off VXProcess behaviour")]
		public bool active = true;

		[Tooltip("Enable runtime applying VXGameobjects to all objects on load")]
        public bool apply_vx_on_load = true;

		[FormerlySerializedAs("_editor_camera")]
        [Tooltip("Collision 'Camera'\nUtilises GameObject Scale, Rotation and Position")]
        [SerializeField]
        private VXCamera editorCamera;

		[Header("Performance")]
		[Tooltip("Apply a fixed framerate for a consistent performance")]
		public bool fixedFrameRate = false;

		[Tooltip("Target framerate when rendering and recording")]
		public int TargetFrameRate = 15;

		[Header("Recording")]
		[Tooltip("Path of recorded frame data. Use for static playback")]
		public string recordingPath = "C:\\Voxon\\Media\\MyCaptures\\framedata";

		[Tooltip("Activate recording on project load")]
		public bool recordOnLoad = false;

		[Tooltip("Capture all vcb into single zip, or as individual frames")]
		public RecordingType recordingStyle = RecordingType.ANIMATION;

        #endregion

        private Vector3 normalLighting = new Vector3();
        private bool lightingUpdated = false;

		#region drawables
		public static List<IDrawable> Drawables = new List<IDrawable>();
		public static List<VXGameObject> Gameobjects = new List<VXGameObject>();
		#endregion

		#region internal_vars
		private Int64 current_frame = 0;
        private string _dll_version = "";
        private VolumetricCamera _camera = new VolumetricCamera();
        static List<string> _logger = new List<string>();
        private List<float> frame_delta = new List<float>();

		bool is_recording = false;

		#endregion

		#region public_vars
		[Header("Logging")]
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

        public Vector3 NormalLight
        {
            get => normalLighting;
            set {
                lightingUpdated = true;
                normalLighting = value;
            }
        }
        #endregion

        #region unity_functions
        private void Awake()
        {
			if (fixedFrameRate)
			{
				QualitySettings.vSyncCount = 0;  // VSync must be disabled
				Application.targetFrameRate = TargetFrameRate;
				Time.captureFramerate = TargetFrameRate;
			}

            awakeDelay = EDITOR_AWAKE_DELAY;

            current_frame = -1; // We haven't started our first frame yet
			Drawables.Clear();
            Gameobjects.Clear();
        }

        void Start() {

            Camera = editorCamera;
            int type = 0;
            startTime = Time.timeAsDouble;

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
/* TODO Clipshape is now independent replace this code for real Helix mode...
                if (editorCamera.helixMode)
                {
                    type += 2;
                 
             
                }
*/
                Runtime.Load();
                #if UNITY_EDITOR
                type += 1; 
                #endif

                Runtime.Initialise(type);
                /*
                if (editorCamera.helixMode)
                {
                    Runtime.SetSimulatorHelixMode(true);
                    Runtime.SetExternalRadius(Camera.helixAspRMax);
                }
 */
                _dll_version = Runtime.GetDLLVersion().ToString().Substring(0, 8);
                
                #if (UNITY_EDITOR)
                    Debug.Log($"Voxon Unity Plugin. Compatible with Unity versions >= 2020");
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

			if (recordOnLoad)
			{
				is_recording = true;
				if(recordingStyle == RecordingType.ANIMATION)
				{
					Voxon.VXProcess.Runtime.StartRecording(recordingPath, TargetFrameRate);
				}
			}
        }

        // Update is called once per frame
        void Update()
        {
			if (debugVxProcess)
            {
                debugVxProcess = false;
            }


            if (restartVXRuntime)
            {
                restartVXRuntime = false;
                Start();
                return;
            }

            if (killVXRuntime)
            {
                killVXRuntime = false;
                restartVXRuntime = true;
                StopAllCoroutines();
                CloseRuntime();
#if UNITY_EDITOR
 UnityEditor.EditorApplication.isPlaying = false;
#endif
                return;
            }

            startTime = Time.timeAsDouble;



            // To prevent  the Unity Editor from crashing we introduce a little delay before a frame start this is because the handle
            // to the voxiebox.DLL can sometimes return NULL and crash the editor if we try to access it to quickly.
#if (UNITY_EDITOR)
                

                  if (awakeDelay > 0 && delayStartUpInEditor == true) { 
                      startDelay = startTime + awakeDelay;
                      awakeDelay = 0;
                  }

                  if (delayStartUpInEditor == false)  startDelay = 0;
#endif

            if (startTime >= startDelay) //
            {
                current_frame++;

                if (!active || Runtime == null)
                {
                    return;
                }

                bool isBreathing = false;

                try
                {
                    isBreathing = Runtime.FrameStart();



                    if (guidelines)
                        Runtime.DrawGuidelines();

                    if (show_version)
                    {
                        Runtime.LogToScreenExt(20, 560, 0xff4000, -1, $"Voxon Unity Plugin");
                        Runtime.LogToScreen(20, 570, $"Voxon Runtime version: {_dll_version}");
                        Runtime.LogToScreen(20, 580, $"Plugin build date: {BuildDate}");
                        Runtime.LogToScreenExt(20, 590, 0x00ffff, -1, $"Compatible with Unity versions >= 2020");

                    }


                    // A camera must always be active while in process
                    if (_camera != null && _camera.Camera == null)
                    {
                        Debug.LogError("No Active VXCamera!");
                        active = false;
                        CloseRuntime();
                        Runtime.FrameEnd();
                        return;
                    }

                    // TODO if Loaded Camera Animation - > Set Camera Transform
                    _camera?.LoadCameraAnim();

                    if (_camera != null && _camera.HasChanged)
                    {

                        _camera?.ForceUpdate();
                    }


                    // TODO If Loaded Capture Playback -> Set Capture Frame Else Draw

                    Draw();

                    // TODO Save Camera Pos
                    _camera?.SaveCameraAnim();
                    // TODO Save Frame
                    if (is_recording && recordingStyle == RecordingType.SINGLE_FRAME)
                    {
                        Runtime.GetVCB(recordingPath, TargetFrameRate);
                    }

                    _camera?.ClearUpdated();


                }
                catch (Exception e)
                {
                    Debug.LogError("Error in Runtime");
                    Debug.LogError(e.Message);
                }
                AudioListener.volume = Runtime.GetVolume();
                // VX quit command; TODO this should be by choice
                if (Runtime.GetKey(0x1) || !isBreathing)
                {
                    CloseRuntime();

                    _camera?.Camera.CloseAnimator();
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
        }

		private void CloseRuntime()
		{
            if (Runtime != null)
            {
                if (is_recording && recordingStyle == RecordingType.ANIMATION)
                {
                    is_recording = false;
                    Runtime.EndRecording();
                }
                Runtime.Shutdown();
                Runtime.Unload();
            }
		}

        private new void OnApplicationQuit()
        {
            if(Runtime != null) {
				CloseRuntime();
			}
        
            base.OnApplicationQuit();
        }

        #endregion

        #region drawing
        private void Draw()
        {
            if(lightingUpdated)
			{
                Debug.Log(normalLighting);
                Runtime.SetNormalLighting(normalLighting.x, normalLighting.y, normalLighting.z);
                lightingUpdated = false;

            }

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
