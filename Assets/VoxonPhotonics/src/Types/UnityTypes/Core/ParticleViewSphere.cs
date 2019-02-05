using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class ParticleViewSphere : ParticleView
    {
        #region Constructors
        public ParticleViewSphere(ParticleModel particle) : this(particle, null) { }
        public ParticleViewSphere(ParticleModel particle, GameObject parent) : base(particle, parent) { }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = model.ParticleCount;

            point3d point;

            for (int idx = 0; idx < particles; ++idx)
            {
                float size = model.GetParticleSize(idx);
                point = model.GetParticle(idx);

                DLL.draw_sphere(ref point, size, 0, model.GetParticleColour(idx));

            }
        }
        #endregion

    }
}
