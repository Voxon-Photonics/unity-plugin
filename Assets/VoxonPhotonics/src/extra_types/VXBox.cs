using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class VXBox : IVXDrawable
    {
        GameObject parent;
        Vector3 _0 = Vector3.zero;
        Vector3 _1 = Vector3.zero;

        float _0x, _0y, _0z;
        float _1x, _1y, _1z;
        int col;
        int fill;

        public VXBox(Vector3 _0, Vector3 _1, int fill, Color32 col) : this(_0, _1, fill, col, null)
        {

        }

        public VXBox(Vector3 _0, Vector3 _1, int fill, Color32 col, GameObject parent)
        {
            set_0(_0);
            set_1(_1);
            set_color(col);
            set_fill(fill);
            set_parent(parent);

            VXProcess._drawables.Add(this);
        }

        public void Draw()
        {
            Voxon.DLL.draw_box(_0x, _0y, _0z, _1x, _1y, _1z, fill, col);
        }

        public void set_color(Color32 _col)
        {
            col = (_col.toInt() & 0xffffff) >> 0;
        }

        public void set_0(Vector3 point)
        {
            _0 = point;

            update_transform();

        }

        public void set_1(Vector3 point)
        {
            _1 = point;

            update_transform();
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
                _0x = in_v.x;
                _0y = -in_v.z;
                _0z = -in_v.y;

                // Rotation shifts where 'lower' value lives
                // What we need to do is convert from a fixed position into a vector
                Vector3 vector = _1 - _0;
                // Then perform rotation based on parent object
                vector = parent.transform.rotation * vector;
                // Finally shift vector back into position based on first position and parent position
                vector = vector + _0 + parent.transform.position;

                // Perform camera transform
                in_v = FMatrix * vector;
                _1x = in_v.x;
                _1y = -in_v.z;
                _1z = -in_v.y;
            }
            else
            {
                in_v = FMatrix * _0;
                _0x = in_v.x;
                _0y = -in_v.z;
                _0z = -in_v.y;

                in_v = FMatrix * _1;
                _1x = in_v.x;
                _1y = -in_v.z;
                _1z = -in_v.y;
            }
            
        }

        private void set_fill(int fill)
        {
            this.fill = fill;
        }
    }
}
