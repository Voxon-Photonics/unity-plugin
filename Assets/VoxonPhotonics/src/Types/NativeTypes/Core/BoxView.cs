using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class BoxView : IDrawable
    {
        BoxModel model = new BoxModel();

        #region Constructors
        public BoxView(Vector3 _0, Vector3 _1, int fill, Color32 col) : this(_0, _1, fill, col, null) { }

        public BoxView(Vector3 _0, Vector3 _1, int fill, Color32 col, GameObject parent)
        {
            // Build Data
            model.Position = new Vector3[] { _0, _1};
            model.Fill = fill;
            model.Parent = parent;

            model.SetColor(col);
            model.Update();

            VXProcess._drawables.Add(this);
        }

        public BoxView(BoxModel box)
        {
            model = box;
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
            Voxon.DLL.draw_box(ref model.Point3dPosition[0], ref model.Point3dPosition[1], model.Fill, model.Color);
        }
        #endregion

    }
}
