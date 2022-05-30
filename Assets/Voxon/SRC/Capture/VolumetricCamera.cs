/* This is a C# namespace. It is a way of grouping classes together. */
using UnityEngine;

namespace Voxon
{
    public class VolumetricCamera
    {
        #region variables
        private Matrix4x4 default_scale = Matrix4x4.Scale(new Vector3(2.0f, 2.0f, 2.0f));
        private VXCamera _camera;

        #endregion

        #region public_functions
        public VXCamera Camera
        {
            get => _camera;

            set
            {
                _camera = value;

                UpdateTransform();
            }
        }

        public Matrix4x4 Transform { get; private set; }

        /// <summary>
        /// Removes hasChanged flag from Camera object
        /// </summary>
        public void ClearUpdated()
        {
            _camera.transform.hasChanged = false;
        }

        public Vector3 EulerAngles
        {
            get => _camera.transform.eulerAngles;

            set
            {
                _camera.transform.eulerAngles = value;
                UpdateTransform();
            }
        }

        /// <summary>
        /// Has Camera has transformed?
        /// </summary>
        /// <returns><c>True</c> if _camera has changed, else <c>False</c></returns>
        public bool HasChanged => _camera.transform.hasChanged;


        /// <summary>
        /// Forced the Camera to update it's transform (matrix) and marks it as having transformed
        /// </summary>
        public void ForceUpdate()
        {
            UpdateTransform();
        }


        /// <summary>
        /// Loads the current Camera transform from the animation sequence
        /// </summary>
		public void LoadCameraAnim()
        {
            Camera.LoadTransform();
        }


        /// <summary>
        /// Attempts to save the current Camera transform (If the camera has transformed) to the animation sequence
        /// </summary>
		public void SaveCameraAnim()
        {
            Camera.SaveTransform(_camera.transform.hasChanged);
        }

        #endregion

        #region private_functions
        /// <summary>
        /// If the camera exists, set the transform to the default scale multiplied by the world to local
        /// matrix of the camera. Then mark the camera has having been updated.
        /// </summary>
        private void UpdateTransform()
        {
            if (!_camera) return;

            _camera.transform.hasChanged = true;

            Transform = default_scale * _camera.transform.worldToLocalMatrix;
        }
        #endregion
    }
}