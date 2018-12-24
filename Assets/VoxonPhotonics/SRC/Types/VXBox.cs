using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class VXBox : IDrawable
    {
        GameObject parent;

        Vector3 _0 = Vector3.zero;
        public Vector3 Position_0
        {
            get { return _0; }
            set { _0 = value; update_transform(); }
        }

        Vector3 _1 = Vector3.zero;
        public Vector3 Position_1
        {
            get { return _1; }
            set { _1 = value; update_transform(); }
        }

        point3d _0p = new point3d(), _1p = new point3d();
        int _col;

        int fill;

        public VXBox(Vector3 _0, Vector3 _1, int fill, Color32 col) : this(_0, _1, fill, col, null)
        {

        }

        public VXBox(Vector3 _0, Vector3 _1, int fill, Color32 col, GameObject parent)
        {
            Position_0 = _0;
            Position_1 = _1;
            set_color(col);
            set_fill(fill);
            set_parent(parent);

            VXProcess._drawables.Add(this);
        }

        public void Draw()
        {
            Voxon.DLL.draw_box(ref _0p, ref _1p, fill, _col);
        }

        public void set_color(Color32 color)
        {
            _col = (color.toInt() & 0xffffff) >> 0;
        }

        public void set_parent(GameObject parent)
        {
            this.parent = parent;
            update_transform();
        }

        public void update_transform()
        {
            Matrix4x4 FMatrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance._camera.transform.worldToLocalMatrix;

            Vector3 in_v;

            if(parent)
            {
                // Perform camera transform
                in_v = FMatrix * (_0 + parent.transform.position);

                _0p = in_v.toPoint3d();

                // Rotation shifts where 'lower' value lives
                // What we need to do is convert from a fixed position into a vector
                Vector3 vector = _1 - _0;
                // Then perform rotation based on parent object
                vector = parent.transform.rotation * vector;
                // Finally shift vector back into position based on first position and parent position
                vector = vector + _0 + parent.transform.position;

                // Perform camera transform
                in_v = FMatrix * vector;
                _1p = in_v.toPoint3d();
            }
            else
            {
                in_v = FMatrix * _0;
                _0p = in_v.toPoint3d();

                in_v = FMatrix * _1;
                _1p = in_v.toPoint3d();
            }
            
        }

        private void set_fill(int fill)
        {
            this.fill = fill;
        }
    }
}
