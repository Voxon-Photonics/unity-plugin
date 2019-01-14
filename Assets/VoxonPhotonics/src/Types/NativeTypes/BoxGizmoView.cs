using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class BoxGizmoView
    {
        BoxGizmoModel boxGizmoModel;

        public BoxGizmoView(BoxModel boxModel)
        {
            boxGizmoModel = new BoxGizmoModel(boxModel);
        }

        public void DrawGizmo()
        {
            boxGizmoModel.Update();
            foreach (Vector3 corner in boxGizmoModel.corners)
            {
                Gizmos.DrawIcon(corner, "Negative.tif", true);
            }

            Gizmos.DrawCube(boxGizmoModel.center, boxGizmoModel.size);
        }
    }
}

