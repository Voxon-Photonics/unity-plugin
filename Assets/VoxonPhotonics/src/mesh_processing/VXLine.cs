using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Voxon
{
    public class VXLine : MonoBehaviour, IVXDrawable
    {
        #region private_variables
        LineRenderer line;
        Voxon.DLL.poltex_t[] points;
        #endregion

        // Use this for initialization
        void Start()
        {
            try
            {
                line = GetComponent<LineRenderer>();
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("(" + name + ") Failed to load suitable Line", E);
                Destroy(this);
            }

            points = new DLL.poltex_t[line.positionCount];

            VXProcess._drawables.Add(this);

            update_transform();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Draw()
        {
            if (!gameObject.activeInHierarchy || tag == "VoxieHide")
            {
                Debug.Log(gameObject.name + ": Skipping");
                return;
            }

            for (int idx = 0; idx < line.positionCount - 1; idx++)
            {
                DLL.draw_line(points[idx].x, points[idx].y, points[idx].z, points[idx+1].x, points[idx + 1].y, points[idx + 1].z, line.startColor.toInt());
            }
            
        }

        void update_transform()
        {
            Matrix4x4 local_to_world = transform.localToWorldMatrix;
            
            Vector3[] vectors = new Vector3[line.positionCount];
            line.GetPositions(vectors);
            VXProcess.ComputeTransform(ref local_to_world, ref vectors, ref points);
        }
    }
}