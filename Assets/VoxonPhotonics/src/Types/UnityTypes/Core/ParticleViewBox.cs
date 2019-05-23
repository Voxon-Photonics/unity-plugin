﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class ParticleViewBox : ParticleView
    {
        #region Constructors
        public ParticleViewBox(ParticleModel particle) : this(particle, null) { }
        public ParticleViewBox(ParticleModel particle, GameObject parent) : base(particle, parent) { }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = model.ParticleCount;

            point3d point, min, max;

            for (int idx = 0; idx < particles; ++idx)
            {
                float size = model.GetParticleSize(idx);
                point = model.GetParticle(idx);

                min.x = point.x - size;
                min.y = point.y - size;
                min.z = point.z - size;

                max.x = point.x + size;
                max.y = point.y + size;
                max.z = point.z + size;

				VXProcess.runtime.DrawBox(ref min, ref max, 2, model.GetParticleColour(idx));
            }
        }
        #endregion

    }
}
