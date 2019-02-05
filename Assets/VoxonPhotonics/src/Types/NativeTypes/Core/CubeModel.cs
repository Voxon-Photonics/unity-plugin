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
    public class CubeModel
    {
        GameObject parent = null;
        Vector3 position = Vector3.zero;
        Vector3 rightVector = Vector3.zero;
        Vector3 forwardVector = Vector3.zero;
        Vector3 downVector = Vector3.zero;

        point3d p3dPosition, p3dRight, p3dForward, p3dDown;

        int col;
        int fill;

        #region data_manipulation
        public void Update()
        {
            Matrix4x4 FMatrix = VXProcess.Instance.Transform;

            Vector3 v;
            if (parent)
            {
                v = (FMatrix * (Position + parent.transform.position));
                p3dPosition = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * Right));
                p3dRight = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * Down));
                p3dDown = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * Forward));
                p3dForward = v.toPoint3d();
            }
            else
            {
                v = FMatrix * Position;
                p3dPosition = v.toPoint3d();

                v = FMatrix * Right;
                p3dRight = v.toPoint3d();

                v = FMatrix * Down;
                p3dDown = v.toPoint3d();

                v = FMatrix * Forward;
                p3dForward = v.toPoint3d();
            }

        }
        #endregion

        #region getters_setters
        public void set_color(Color32 _col)
        {
            col = (_col.toInt() & 0xffffff) >> 0;
        }

        public Vector3 Position
        {
            set
            {
                this.position = value;
                Update();
            }

            get
            {
                return position;
            }

        }

        public Vector3 Down
        {
            set
            {
                this.downVector = value;
                Update();
            }
            get
            {
                return downVector;
            }
        }

        public Vector3 Right
        {
            set
            {
                this.rightVector = value;
                Update();
            }

            get
            {
                return rightVector;
            }
        }

        public Vector3 Forward
        {
            set
            {
                this.forwardVector = value;
                Update();
            }

            get
            {
                return forwardVector;
            }
        }

        public GameObject Parent
        {
            set
            {
                this.parent = value;
                Update();
            }

            get
            {
                return parent;
            }
        }

        public int Fill
        {
            set { this.fill = value; }
            get { return fill; }
        }

        public int Color
        {
            get { return col; }
        }

        public point3d P3DPosition
        {
            get { return this.p3dPosition; }
        }

        public point3d P3DRight
        {
            get { return this.p3dRight; }
        }

        public point3d P3DForward
        {
            get { return this.p3dForward; }
        }

        public point3d P3DDown
        {
            get { return this.p3dDown; }
        }
        #endregion
    }
}
