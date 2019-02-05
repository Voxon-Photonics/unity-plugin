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
    public class LineModel
    {
        GameObject parent;
        Vector3[] vPoints;
        point3d[] p3dPoints;
        int colour;

        #region data_manipulation
        public void Update()
        {
            if(p3dPoints == null || p3dPoints.Length != vPoints.Length) p3dPoints = new point3d[vPoints.Length];

            Matrix4x4 mat = Matrix4x4.identity;
            if (parent)
            {
                mat = parent.transform.localToWorldMatrix;
            }

            VXProcess.ComputeTransform(ref mat, ref vPoints, ref p3dPoints);
        }
        #endregion

        #region getters_setters
        public Vector3[] Points
        {
            get { return vPoints; }
            set { vPoints = value; Update(); }
        }

        public point3d[] Point3dPoints
        {
            get { return p3dPoints; }
        }

        public GameObject Parent
        {
            get { return parent; }
            set { parent = value; Update(); }
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
