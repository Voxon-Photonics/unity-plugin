using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public enum flags
    {
        dots = 0,
        lines = 1,
        surfaces = 2,
        solid = 3
    };

    public interface IDrawable
    {
        void Draw();
    }
}
