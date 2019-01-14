using UnityEngine;

namespace Voxon
{
    public class VolumetricCamera
    {
        #region variables
        private Matrix4x4 default_scale = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f));
        private GameObject _camera;
        private Matrix4x4 _transform = new Matrix4x4();
        #endregion

        #region public_functions
        public GameObject Camera
        {
            get
            {
                return _camera;
            }
            set
            {
                _camera = value;
                UpdateTransform();
            }
        }

        public Matrix4x4 Transform
        {
            get
            {
                return _transform;
            }
        }

        public void ClearUpdated()
        {
            _camera.transform.hasChanged = false;
        }

        public Vector3 EulerAngles
        {
            get
            {
                return _camera.transform.eulerAngles;
            }

            set
            {
                _camera.transform.eulerAngles = value;
                UpdateTransform();
            }
        }

        public bool HasChanged
        {
            get { return _camera.transform.hasChanged; } 
        }
        #endregion

        #region private_functions
        private void UpdateTransform()
        {
            if(_camera)
            {
                _camera.transform.hasChanged = true;
                _transform = default_scale * _camera.transform.worldToLocalMatrix;
            }
        }
        #endregion
    }
}