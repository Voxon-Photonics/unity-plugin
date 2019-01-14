using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Voxon
{
    public class VXTextComponent : MonoBehaviour, IDrawable
    {
        point3d pr, pd, pp;

        [Tooltip("right vector - length is size of single character")]
        public Vector3 _pr = new Vector3(0.1f, 0.0f, 0.0f);
        [Tooltip("down vector - length is height of character")]
        public Vector3 _pd = new Vector3(0.0f, 0.5f, 0.0f);
        [Tooltip("top-left-up corner of first character")]
        public Vector3 _pp = new Vector3(-0.75f, 0.5f, 0.0f);
        [Tooltip("text colour")]
        public int _color = 0xffffff;

        private static System.Text.Encoding enc = System.Text.Encoding.ASCII;

        public string text = "";
        byte[] ts;
        // Use this for initialization
        void Start()
        {
            pr.x = _pr.x;
            pr.y = _pr.y;
            pr.z = _pr.z;

            pd.x = _pd.x;
            pd.y = _pd.y;
            pd.z = _pd.z;

            pp.x = _pp.x;
            pp.y = _pp.y;
            pp.z = _pp.z;

            SetString(text);
            UpdateLocation();

            VXProcess._drawables.Add(this);
        }

        public void SetString(string Text)
        {
            text = Text;
            // Get Char Values for String
            byte[] tmp = enc.GetBytes(Text);
            ts = new byte[tmp.Length+1];
            tmp.CopyTo(ts, 0);
            // Append 0 to end string
            ts[tmp.Length] = 0;
        }

        public void UpdateLocation()
        {
            Matrix4x4 Matrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance.Camera.transform.worldToLocalMatrix * transform.localToWorldMatrix;

            Vector3 pos = Matrix * transform.position;

            pp = pos.toPoint3d();
        }

        public void SetCharWidth(point3d width)
        {
            pd = width;
        }

        public void SetCharWidth(Vector3 width)
        {
            SetCharWidth(width.toPoint3d());
        }

        public void SetCharHeight(point3d height)
        {
            pr = height;
        }

        public void SetCharHeight(Vector3 height)
        {
            SetCharHeight(height.toPoint3d());
        }

        /// <summary>  
        ///  Draw the drawable mesh; Uses Capture Volume's transform to determine if play space has changed
        ///  Animated meshes are set to redraw every frame while statics only redrawn on them or the volume
        ///  changing transform.
        ///  </summary>
        public void Draw()
        {
            if (!gameObject.activeInHierarchy || tag == "VoxieHide")
            {
                Debug.Log(gameObject.name + ": Skipping");
                return;
            }

            Voxon.DLL.draw_letters(ref pp, ref pr, ref pd, _color, ts);
        }

        private void OnDestroy()
        {
            try
            {
                ts = new byte[1];
                ts[0] = 0;

                // Remove ourselves from Draw cycle
                VXProcess._drawables.Remove(this);
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Destroying " + gameObject.name, E);
            }
        }
    }
}