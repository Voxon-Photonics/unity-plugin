using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class SphereView : IDrawable
    {
        SphereModel model = new SphereModel();

        #region Constructors
        public SphereView(Vector3 position, float radius, int fill, Color32 col) : this(position, radius, fill, col, null) { }

        public SphereView(Vector3 position, float radius, int fill, Color32 col, GameObject parent)
        {
            // Build Data
            model.Position = new Vector3[] { position };
            model.Fill = fill;
            model.Parent = parent;
            model.Radius = radius;

            model.SetColor(col);
            model.Update();

            VXProcess._drawables.Add(this);
        }

        public SphereView(SphereModel sphere)
        {
            model = sphere;
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
            Voxon.DLL.draw_sphere(ref model.Point3dPosition[0], model.Radius, model.Fill, model.Color);
        }
        #endregion

    }
}
