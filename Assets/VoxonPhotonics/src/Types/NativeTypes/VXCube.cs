using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class VXCube : IDrawable
    {
        CubeModel cubeModel;

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col) : this(pp, pr, pf, pd, fill, col, null)
        {
        }

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col, GameObject parent)
        {
            cubeModel = new CubeModel();

            cubeModel.Position = pp;
            cubeModel.Right = pr;
            cubeModel.Forward = pf;
            cubeModel.Down = pd;

            cubeModel.Fill = fill;
            cubeModel.Parent = parent;
            cubeModel.set_color(col);

            VXProcess._drawables.Add(this);
        }

        public void Update()
        {
            cubeModel.Update();
        }

        public void Draw()
        {
            point3d[] point3D = { cubeModel.P3DPosition, cubeModel.P3DRight, cubeModel.P3DDown, cubeModel.P3DForward };
            Voxon.DLL.draw_cube(ref point3D[0], ref point3D[1], ref point3D[2], ref point3D[3], cubeModel.Fill, cubeModel.Color);
        }
    }
}
