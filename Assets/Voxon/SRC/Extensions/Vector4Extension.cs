﻿using UnityEngine;
// ReSharper disable CheckNamespace

public static class Vector4Extension
{

    public static Voxon.point3d ToPoint3d(this Vector4 v4)
    {
        Voxon.point3d p3d = new Voxon.point3d {x = v4.x, y = -v4.z, z = -v4.y};

        return p3d;
    }
}