using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class VXVoxel : IVXDrawable
    {
        Vector3 vector = Vector3.zero;

        float _x, _y, _z;
        Int32 col;

        public VXVoxel(Vector3 vector, Color32 col)
        {
            set_vector(vector);
            set_color(col);

            VXProcess._drawables.Add(this);
        }

        public void Draw()
        {
            Voxon.DLL.draw_voxel(_x, _y, _z, col);
        }
            
        public void set_color(Color32 _col)
        {
            col = (_col.toInt() & 0xffffff) >> 0;
        }

        public void set_vector(Vector3 point)
        {
            vector = point;
            update_transform();
        }

        public void update_transform()
        {
            Matrix4x4 FMatrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance._camera.transform.worldToLocalMatrix;

            Vector3 in_v = FMatrix * vector;
            _x = in_v.x;
            _y = -in_v.z;
            _z = -in_v.y;
        }
    }
}