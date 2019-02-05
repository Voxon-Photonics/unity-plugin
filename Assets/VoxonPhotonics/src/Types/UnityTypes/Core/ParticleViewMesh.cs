using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    public class ParticleViewMesh : ParticleView
    {
        RegisteredMesh mesh;
        poltex_t[] transformed_mesh;

        int draw_flags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer

        #region Constructors
        public ParticleViewMesh(ParticleModel particle, Mesh mesh) : this(particle, null, mesh) { }
        public ParticleViewMesh(ParticleModel particle, GameObject parent, Mesh in_mesh) : base(particle, parent)
        {
            try
            {
                mesh = MeshRegister.Instance.get_registed_mesh(ref in_mesh);
                transformed_mesh = new poltex_t[mesh.vertex_count];
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Loading Mesh: " + in_mesh.name, E);
            }
        }
        #endregion

        #region drawing
        public override void Draw()
        {
            int particles = model.ParticleCount;

            Matrix4x4 Matrix = Matrix4x4.identity;

            for (int idx = 0; idx < particles; ++idx)
            {

                // Unity Style
                Matrix = VXProcess.Instance.Transform * model.GetMatrix(idx);

                mesh.compute_transform_cpu(Matrix, ref transformed_mesh);

                for (int idy = mesh.submesh_count - 1; idy >= 0; --idy)
                {
                    DLL.draw_untextured_mesh(transformed_mesh, mesh.vertex_count, mesh.indices[idy], mesh.index_counts[idy], draw_flags, model.GetParticleColour(idx));
                }
            }
        }
        #endregion

    }
}
