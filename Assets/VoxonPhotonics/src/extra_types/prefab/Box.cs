using UnityEngine;

namespace Voxon
{
    public class Box : MonoBehaviour
    {
        public GameObject parent;
        public Vector3 _0 = Vector3.zero;
        public Vector3 _1 = Vector3.zero;
        public int fill = 1;
        public Color32 box_col = Color.white;

        VXBox box;
        // Use this for initialization
        void Start()
        {
            box = new VXBox(_0, _1, fill, box_col);
        }

        private void Update()
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = box_col;
            if (parent)
            {
                Vector3 tmp_0 = _0 + parent.transform.position;

                Vector3 tmp_1 = parent.transform.rotation * (_1 - _0);
                tmp_1 = (tmp_1 + _0 + parent.transform.position);
                tmp_1.x = tmp_1.x * parent.transform.localScale.x;
                tmp_1.y = tmp_1.y * parent.transform.localScale.y;
                tmp_1.z = tmp_1.z * parent.transform.localScale.z;

                Vector3 _00_Corner = new Vector3(tmp_0.x, tmp_0.y, tmp_1.z);
                Vector3 _01_Corner = new Vector3(tmp_0.x, tmp_1.y, tmp_0.z);
                Vector3 _02_Corner = new Vector3(tmp_1.x, tmp_0.y, tmp_0.z);
                Vector3 _10_Corner = new Vector3(tmp_1.x, tmp_1.y, tmp_0.z);
                Vector3 _11_Corner = new Vector3(tmp_1.x, tmp_0.y, tmp_1.z);
                Vector3 _12_Corner = new Vector3(tmp_0.x, tmp_1.y, tmp_1.z);

                // Markers
                Gizmos.DrawIcon(tmp_0, "Forward.tif", true);
                Gizmos.DrawIcon(tmp_1, "Negative.tif", true);


                Gizmos.DrawIcon(_00_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_01_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_02_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_10_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_11_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_12_Corner, "Negative.tif", true);

                // Lines
                Vector3 size = new Vector3( Mathf.Abs(tmp_1.x - tmp_0.x),
                                            Mathf.Abs(tmp_1.y - tmp_0.y),
                                            Mathf.Abs(tmp_1.z - tmp_0.z));

                Gizmos.DrawCube((tmp_0 + tmp_1) / 2, size);
            }
            else
            {
                Vector3 _00_Corner = new Vector3(_0.x, _0.y, _1.z);
                Vector3 _01_Corner = new Vector3(_0.x, _1.y, _0.z);
                Vector3 _02_Corner = new Vector3(_1.x, _0.y, _0.z);
                Vector3 _10_Corner = new Vector3(_1.x, _1.y, _0.z);
                Vector3 _11_Corner = new Vector3(_1.x, _0.y, _1.z);
                Vector3 _12_Corner = new Vector3(_0.x, _1.y, _1.z);

                // Markers
                Gizmos.DrawIcon(_0, "Forward.tif", true);
                Gizmos.DrawIcon(_1, "Negative.tif", true);


                Gizmos.DrawIcon(_00_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_01_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_02_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_10_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_11_Corner, "Negative.tif", true);
                Gizmos.DrawIcon(_12_Corner, "Negative.tif", true);

                // Lines
                Vector3 size = new Vector3(Mathf.Abs(_1.x - _0.x),
                                            Mathf.Abs(_1.y - _0.y),
                                            Mathf.Abs(_1.z - _0.z));
                Gizmos.DrawCube((_0 + _1) / 2, _1 - _0);
            }
            


        }

    }
}
