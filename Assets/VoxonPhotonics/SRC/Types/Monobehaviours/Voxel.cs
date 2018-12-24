using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct vboi
{
    public Vector3 vector;
    public Color voxel_color;
}

public class Voxel : MonoBehaviour
{
    [SerializeField]
    public List<vboi> voxel_params = new List<vboi>();
    //public Vector3 vector = Vector3.zero;
    //public Color voxel_color = Color.white;

    List<Voxon.VXVoxel> voxels = new List<Voxon.VXVoxel>();
    // Use this for initialization
    void Start()
    {
        foreach(vboi vp in voxel_params)
        {
            voxels.Add(new Voxon.VXVoxel(vp.vector, vp.voxel_color));
        }
    }

    private void OnDrawGizmos()
    {
        foreach (vboi vp in voxel_params)
        {
            Gizmos.DrawIcon(vp.vector, "Light Gizmo.tiff", true);
        }
        
    }

}
