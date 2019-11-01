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
            // Debug.Log("Point: " + point.ToString());
            this._vector = point;
            update_transform();
        }

        public void update_transform()
        {
            Matrix4x4 fMatrix = VXProcess.Instance.Transform;
            if (fMatrix == Matrix4x4.zero)
            {
                Debug.Log("EMPTY TRANSFORM");
            }
            else
            {
                Vector3 inV = fMatrix * _vector;
                _position = inV.ToPoint3d();    
            }
            
        }
    }
}