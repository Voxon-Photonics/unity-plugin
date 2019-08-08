using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Voxon.Examples.Animation
{
    [ExecuteInEditMode]
    public class RandomVoxels : MonoBehaviour
    {
        public int voxelCount = 1000;
        //public Vector3 vector = Vector3.zero;
        //public Color voxel_color = Color.white;

        private List<VXVoxel> _voxels = new List<VXVoxel>();

        // Use this for initialization
        private void Start()
        {
            for(var idx = 0; idx < voxelCount; idx++)
            {
                _voxels.Add(new VXVoxel(new Vector3(Random.Range(-5,5), Random.Range(-2, 2), Random.Range(-5, 5)), Color.white));
            }
        }
    }
}
