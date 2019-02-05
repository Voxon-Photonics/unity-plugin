using UnityEngine;

namespace Voxon
{
    public class BoxGizmoModel
    {
        public BoxModel box;
        public Vector3[] corners = new Vector3[6];

        public Vector3 size;
        public Vector3 center;

        #region constructor
        public BoxGizmoModel(BoxModel box)
        {
            this.box = box;
            Update();
        }
        #endregion

        #region data_manipulation
        public void Update()
        {
            if (box.Parent)
            {
                UpdateParent();
            }
            else
            {
                UpdateNoParent();
            }
        }

        private void UpdateParent()
        {
            Vector3 tmp_0 = box.Position[0] + box.Parent.transform.position;
            Vector3 tmp_1 = box.Parent.transform.rotation * (box.Position[1] - box.Position[0]);

            tmp_1 = (tmp_1 + box.Position[0] + box.Parent.transform.position);
            tmp_1.x = tmp_1.x * box.Parent.transform.localScale.x;
            tmp_1.y = tmp_1.y * box.Parent.transform.localScale.y;
            tmp_1.z = tmp_1.z * box.Parent.transform.localScale.z;

            corners[0] = new Vector3(tmp_0.x, tmp_0.y, tmp_1.z);
            corners[1] = new Vector3(tmp_0.x, tmp_1.y, tmp_0.z);
            corners[2] = new Vector3(tmp_1.x, tmp_0.y, tmp_0.z);
            corners[3] = new Vector3(tmp_1.x, tmp_1.y, tmp_0.z);
            corners[4] = new Vector3(tmp_1.x, tmp_0.y, tmp_1.z);
            corners[5] = new Vector3(tmp_0.x, tmp_1.y, tmp_1.z);

            size = new Vector3(Mathf.Abs(box.Position[1].x - box.Position[0].x),
                                Mathf.Abs(box.Position[1].y - box.Position[0].y),
                                Mathf.Abs(box.Position[1].z - box.Position[0].z));

            center = (tmp_0 + tmp_1) * 0.5f;
        }

        private void UpdateNoParent()
        {
            corners[0] = new Vector3(box.Position[0].x, box.Position[0].y, box.Position[0].z);
            corners[1] = new Vector3(box.Position[1].x, box.Position[1].y, box.Position[1].z);
            // corners[2] = new Vector3(box.Position[1].x, box.Position[0].y, box.Position[1].z);
            // corners[3] = new Vector3(box.Position[1].x, box.Position[1].y, box.Position[0].z);
            // corners[4] = new Vector3(box.Position[1].x, box.Position[0].y, box.Position[1].z);
            // corners[5] = new Vector3(box.Position[0].x, box.Position[1].y, box.Position[1].z);

            size = new Vector3(Mathf.Abs(box.Position[1].x - box.Position[0].x),
                                            Mathf.Abs(box.Position[1].y - box.Position[0].y),
                                            Mathf.Abs(box.Position[1].z - box.Position[0].z));

            center = (box.Position[0] + box.Position[1]) * 0.5f;
        }
        #endregion
    }
}