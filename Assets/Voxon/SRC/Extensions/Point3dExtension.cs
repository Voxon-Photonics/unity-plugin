using UnityEngine;
// ReSharper disable CheckNamespace

public static class Point3dExtension{
    public static Vector3 ToVector3(this Voxon.point3d p3d)
    {
        return new Vector3(p3d.x, p3d.y, p3d.z);
    }
}
