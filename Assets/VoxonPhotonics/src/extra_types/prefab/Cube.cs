using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class Cube : MonoBehaviour
    {
        public GameObject parent;
        public Vector3 top_left_up = Vector3.zero;
        public Vector3 right_vector = Vector3.zero;
        public Vector3 down_vector = Vector3.zero;
        public Vector3 forward_vector = Vector3.zero;

        public int fill = 1;
        public Color32 box_col = Color.white;

        VXCube cube;

        // tmp values
        Vector3 top_right = Vector3.zero;
        Vector3 top_down = Vector3.zero;
        Vector3 top_forward = Vector3.zero;
        Vector3 top_far = Vector3.zero;

        Vector3 down_right = Vector3.zero;
        Vector3 down_forward = Vector3.zero;
        Vector3 down_far = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            cube = new VXCube(top_left_up, right_vector, forward_vector, down_vector, fill, box_col, parent);
        }

        private void Update()
        {
            cube.update_transform();
        }

        private void OnDrawGizmos()
        {
            if(parent)
            {
                top_left_up = parent.transform.position;

                Vector3 t_right = parent.transform.rotation * right_vector;
                Vector3 t_down = parent.transform.rotation * down_vector;
                Vector3 t_forward = parent.transform.rotation * forward_vector;

                top_right = top_left_up + t_right;
                top_down = top_left_up + t_down;
                top_forward = top_left_up + t_forward;
                top_far = top_left_up + t_right + t_forward;

                down_right = top_down + t_right;
                down_forward = top_down + t_forward;
                down_far = top_down + t_right + t_forward;
            }
            else
            {
                top_right = top_left_up + right_vector;
                top_down = top_left_up + down_vector;
                top_forward = top_left_up + forward_vector;
                top_far = top_left_up + right_vector + forward_vector;

                down_right = top_down + right_vector;
                down_forward = top_down + forward_vector;
                down_far = top_down + right_vector + forward_vector;
            }

            // Markers
            Gizmos.color = new Color(1 - box_col.r, 1 - box_col.b, 1 - box_col.g);
            Gizmos.DrawIcon(top_left_up, "Exclaim.tif", true);

            Gizmos.color = box_col;
            Gizmos.DrawIcon(top_right, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(top_forward, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(top_down, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(top_far, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(down_far, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(down_right, "Light Gizmo.tiff", true);
            Gizmos.DrawIcon(down_forward, "Light Gizmo.tiff", true);

            // Lines
            Gizmos.DrawLine(top_left_up, top_right);
            Gizmos.DrawLine(top_left_up, top_down);
            Gizmos.DrawLine(top_left_up, top_forward);
            Gizmos.DrawLine(down_far, down_forward);
            Gizmos.DrawLine(down_far, down_right);
            Gizmos.DrawLine(down_far, top_far);
        }

    }
}
