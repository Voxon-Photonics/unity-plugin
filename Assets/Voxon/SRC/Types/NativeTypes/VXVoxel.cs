using System;
using UnityEngine;

namespace Voxon
{
    public class VXVoxel : IDrawable
    {
        private Vector3 _vector = Vector3.zero;

        private point3d _position;
        private Int32 _col;

        public VXVoxel(Vector3 vector, Color32 col)
        {
            set_vector(vector);
            set_color(col);

            VXProcess.Drawables.Add(this);
        }

        public void Draw()
        {
			VXProcess.Runtime.DrawVoxel(ref _position, _col);
        }
            
        public void set_color(Color32 col)
        {
            _col = (col.ToInt() & 0xffffff) >> 0;
        }

        public void set_vector(Vector3 point)
        {
            _vector = point;
            update_transform();
        }

        public void update_transform()
        {
            Matrix4x4 fMatrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance.Camera.transform.worldToLocalMatrix;

            Vector3 inV = fMatrix * _vector;
            _position = inV.ToPoint3d();
        }
    }
}