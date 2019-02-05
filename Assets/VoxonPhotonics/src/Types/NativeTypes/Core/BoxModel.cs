using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class BoxModel
    {
        GameObject parent;
        Vector3[] vPosition = new Vector3[2];
        point3d[] p3dPosition = new point3d[2];
        int colour;
        int fill;

        #region data_manipulation
        public void Update()
        {
            Matrix4x4 mat = Matrix4x4.identity;
            if (parent)
            {
                mat = parent.transform.localToWorldMatrix;
            }

            VXProcess.ComputeTransform(ref mat, ref vPosition, ref p3dPosition);
        }
        #endregion

        #region getters_setters
        public Vector3[] Position
        {
            get { return vPosition; }
            set { vPosition = value; Update(); }
        }

        public point3d[] Point3dPosition
        {
            get { return p3dPosition; }
        }

        public GameObject Parent
        {
            get { return parent; }
            set { parent = value; Update(); }
        }

        public int Fill
        {
            set { fill = value; }
            get { return fill; }
        }

        public void SetColor(Color32 color)
        {
            colour = (color.toInt() & 0xffffff) >> 0;
        }

        public int Color
        {
            get { return colour; }
        }
        #endregion
    }
}
