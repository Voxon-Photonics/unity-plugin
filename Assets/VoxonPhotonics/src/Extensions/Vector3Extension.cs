using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension{

	public static Voxon.DLL.point3d toPoint3d(this Vector3 v3)
    {
        Voxon.DLL.point3d p3d = new Voxon.DLL.point3d();
        p3d.x = v3.x;
        p3d.y = -v3.z;
        p3d.z = -v3.y;

        return p3d; 
    }
}
