using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class VXCube : IVXDrawable
    {
        GameObject parent = null;
        Vector3 pp = Vector3.zero;
        Vector3 pr = Vector3.zero;
        Vector3 pf = Vector3.zero;
        Vector3 pd = Vector3.zero;

        DLL.point3d p, r, f, d;

        int col;
        int fill;

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col) : this(pp, pr, pf, pd, fill, col, null)
        {
        }

        public VXCube(Vector3 pp, Vector3 pr, Vector3 pf, Vector3 pd, int fill, Color32 col, GameObject parent)
        {
            set_pp(pp);
            set_pr(pr);
            set_pf(pf);
            set_pd(pd);

            set_fill(fill);
            set_color(col);
            set_parent(parent);

            VXProcess._drawables.Add(this);
        }

        public void Draw()
        {
            Voxon.DLL.draw_cube(ref p,ref r, ref d, ref f, fill, col);
        }

        public void set_color(Color32 _col)
        {
            col = (_col.toInt() & 0xffffff) >> 0;
        }

        public void set_pp(Vector3 point)
        {
            this.pp = point;
            update_transform();

        }

        public void set_pd(Vector3 point)
        {
            this.pd = point;
            update_transform();

        }

        public void set_pr(Vector3 point)
        {
            this.pr = point;
            update_transform();

        }

        public void set_pf(Vector3 point)
        {
            this.pf = point;
            update_transform();
        }

        public void set_parent(GameObject parent)
        {
            this.parent = parent;
            update_transform();
        }


        public void update_transform()
        {
            Matrix4x4 FMatrix = Matrix4x4.Scale(new Vector3(1.0f, 0.4f, 1.0f)) * VXProcess.Instance._camera.transform.worldToLocalMatrix;

            Vector3 v;
            if (parent)
            {
                v = (FMatrix * (pp + parent.transform.position));
                p = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * pr));
                r = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * pd));
                d = v.toPoint3d();

                v = (FMatrix * (parent.transform.rotation * pf));
                f = v.toPoint3d();
            }
            else
            {
                v = FMatrix * pp;
                p = v.toPoint3d();

                v = FMatrix * pr;
                r = v.toPoint3d();

                v = FMatrix * pd;
                d = v.toPoint3d();

                v = FMatrix * pf;
                f = v.toPoint3d();
            }
            
        }

        private void set_fill(int fill)
        {
            this.fill = fill;
        }
    }
}
