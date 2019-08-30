﻿using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Voxon
{
    public class VolumetricCamera
    {
        #region variables
        private Matrix4x4 default_scale = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f));
        private GameObject _camera;

        #endregion

        #region public_functions
        public GameObject Camera
        {
            get => _camera;

            set
            {
                _camera = value;
                UpdateTransform();
            }
        }

        public Matrix4x4 Transform { get; private set; }

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

        public bool HasChanged => _camera.transform.hasChanged;

        public void ForceUpdate()
        {
            UpdateTransform();
        }
        #endregion

        #region private_functions
        private void UpdateTransform()
        {
            if (!_camera) return;
            
            _camera.transform.hasChanged = true;
            Transform = default_scale * _camera.transform.worldToLocalMatrix;
        }
        #endregion
    }
}