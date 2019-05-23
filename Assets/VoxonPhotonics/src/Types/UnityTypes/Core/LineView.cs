using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class LineView : IDrawable
    {
        LineModel model = new LineModel();

        #region Constructors
        public LineView(Vector3[] points, Color32 col) : this(points, col, null) { }

        public LineView(Vector3[] points, Color32 col, GameObject parent)
        {
            // Build Data
            model.Points = points;
            model.Parent = parent;

            model.SetColor(col);
            model.Update();

            VXProcess._drawables.Add(this);
        }

        public LineView(LineModel line)
        {
            model = line;
            VXProcess._drawables.Add(this);
            model.Update();
        }

        public void Destroy()
        {
            VXProcess._drawables.Remove(this);
        }
        #endregion

        #region drawing
        public void Draw()
        {
            model.Update();
            point3d[] points = model.Point3dPoints;
            int count = points.Length - 1;

            for (int idx = 0; idx < count; idx++)
            {
				VXProcess.runtime.DrawLine(ref points[idx], ref points[idx + 1], model.Color);
            }
        }
        #endregion

    }
}