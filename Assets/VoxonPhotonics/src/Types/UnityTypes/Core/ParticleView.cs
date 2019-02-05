using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public abstract class ParticleView : IDrawable
    {
        protected ParticleModel model = new ParticleModel();

        #region Constructors
        public ParticleView() { }

        public ParticleView(ParticleModel particle) : this(particle, null) { }

        public ParticleView(ParticleModel particle, GameObject parent)
        {
            model = particle;
            VXProcess._drawables.Add(this);
            model.Update();
        }

        public void Destroy()
        {
            VXProcess._drawables.Remove(this);
        }
        #endregion

        #region drawing
        public abstract void Draw();
        #endregion

    }
}
