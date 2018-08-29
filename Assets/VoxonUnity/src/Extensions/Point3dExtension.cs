using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Point3dExtension{
    public static Vector3 toVector3(this Voxon.DLL.point3d p3d)
    {
        return new Vector3(p3d.x, p3d.y, p3d.z);
    }

}
