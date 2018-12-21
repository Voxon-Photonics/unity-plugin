using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RandomVoxels : MonoBehaviour
{
    public int voxelCount = 1000;
    //public Vector3 vector = Vector3.zero;
    //public Color voxel_color = Color.white;

    List<Voxon.VXVoxel> voxels = new List<Voxon.VXVoxel>();

    // Use this for initialization
    void Start()
    {
        for(int idx = 0; idx < voxelCount; idx++)
        {
            //voxels.Add(new Voxon.VXVoxel(Vector3.zero, Color.white));
            voxels.Add(new Voxon.VXVoxel(new Vector3(Random.Range(-5,5), Random.Range(-2, 2), Random.Range(-5, 5)), Color.white));
        }
    }

    /*
    private void OnDrawGizmos()
    {
        foreach (vboi vp in voxel_params)
        {
            Gizmos.DrawIcon(vp.vector, "Light Gizmo.tiff", true);
        }

    }
    */

}
