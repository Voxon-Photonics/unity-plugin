using UnityEngine;

public static class Vector4Extension
{

    public static Voxon.point3d toPoint3d(this Vector4 v4)
    {
        Voxon.point3d p3d = new Voxon.point3d();
        p3d.x = v4.x;
        p3d.y = -v4.z;
        p3d.z = -v4.y;

        return p3d;
    }
}
