using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Runtime.InteropServices;



namespace Voxon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VXParticle : MonoBehaviour, IDrawable
    {
        public enum ParticleStyle
        {
            Billboard,
            Box,
            Mesh,
            Sphere
        };

        public ParticleStyle particle_style = ParticleStyle.Billboard;

        private int draw_flags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer

        private ParticleSystem m_particleSystem;
        ParticleSystem.Particle[] m_Particles;
        int n_Particles;

        Matrix4x4 Matrix;

        RegisteredMesh mesh;

        // DLL Version (Source is mesh.vertices)
        poltex_t[][] destinations;

        ParticleSystemRenderMode renderMode;

        // Use this for initialization
        void Start() {
            m_particleSystem = GetComponent<ParticleSystem>();
            m_Particles = new ParticleSystem.Particle[m_particleSystem.main.maxParticles];

            var main = m_particleSystem.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            renderMode = GetComponent<ParticleSystemRenderer>().renderMode;
            if(renderMode == ParticleSystemRenderMode.Mesh)
            {
                load_meshes();

                // Particles destination structures
                destinations = new poltex_t[m_particleSystem.main.maxParticles][];
                for (int idx = m_particleSystem.main.maxParticles - 1; idx >= 0; --idx)
                {
                    destinations[idx] = new poltex_t[mesh.vertex_count];
                }
            }

            VXProcess._drawables.Add(this);
        }

        public void Draw()
        {
            // TODO : The IsAlive check may cause problems in the future, leaving for now due to safety
            if(!m_particleSystem || !m_particleSystem.IsAlive() || !gameObject.activeInHierarchy)
            {
                Debug.Log(gameObject.name + ": Skipping");
                return;
            }

            switch (particle_style)
            {
                case ParticleStyle.Billboard:
                    Draw_Billboard();
                    break;
                case ParticleStyle.Box:
                    Draw_Box();
                    break;
                case ParticleStyle.Mesh:
                    if (renderMode == ParticleSystemRenderMode.Mesh)
                    {
                        Draw_Mesh();
                    }
                    break;
                case ParticleStyle.Sphere:
                    Draw_Sphere();
                    break;
                default:
                    break;

            }
        }

        public void Draw_Billboard()        
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            point3d point, min, max;

            for (int idx = 0; idx < n_Particles; ++idx)
            {
                float size = m_Particles[idx].GetCurrentSize(m_particleSystem) * 0.05f;
                point = (VXProcess.Instance.Transform * m_Particles[idx].position).toPoint3d();
                min.x = point.x - size;
                min.y = point.y;
                min.z = point.z - size;

                max.x = point.x + size;
                max.y = point.y;
                max.z = point.z + size;

                DLL.draw_box(ref min, ref max, 2, (m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt());
            }
        }

        public void Draw_Box()
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            point3d point, min, max;

            for (int idx = 0; idx < n_Particles; ++idx)
            {
                float size = m_Particles[idx].GetCurrentSize(m_particleSystem) * 0.05f;
                point = (VXProcess.Instance.Transform * m_Particles[idx].position).toPoint3d();
                min.x = point.x - size;
                min.y = point.y - size;
                min.z = point.z - size;

                max.x = point.x + size;
                max.y = point.y + size;
                max.z = point.z + size;

                DLL.draw_box(ref min, ref max, 2, (m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt());
            }
        }

        public void Draw_Mesh()
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            Matrix4x4 FMatrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance.Camera.transform.worldToLocalMatrix;

            for (int idx = 0; idx < n_Particles; ++idx)
            {
                // Unity Style 
                Matrix.SetTRS(m_Particles[idx].position, Quaternion.Euler(m_Particles[idx].rotation3D), m_Particles[idx].GetCurrentSize3D(m_particleSystem));

                Matrix = FMatrix * Matrix;

                mesh.compute_transform_cpu(Matrix, ref destinations[idx]);
                
                for (int idy = mesh.submesh_count-1; idy >= 0; --idy)                {
                    DLL.draw_untextured_mesh(destinations[idx], mesh.vertex_count, mesh.indices[idy], mesh.index_counts[idy], draw_flags, (m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt());
                }
            }
        }

        // Draw Sphere
        public void Draw_Sphere()
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            point3d point;

            for (int idx = 0; idx < n_Particles; ++idx)
            {
                float size = m_Particles[idx].GetCurrentSize(m_particleSystem) * 0.05f;
                point = (VXProcess.Instance.Transform * m_Particles[idx].position).toPoint3d();

                DLL.draw_sphere(ref point, size, 0, ((m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt() & 0xffffff) >> 0);

            }
        }

        private void load_meshes()
        {
            try
            {
                Mesh Umesh = GetComponent<ParticleSystemRenderer>().mesh;
                mesh = MeshRegister.Instance.get_registed_mesh(ref Umesh);
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Loading Mesh: " + gameObject.name, E);
            }
        }

        // Use destroy to free gpu data
        void OnDestroy()
        {
            try
            {
                // Remove ourselves from Draw cycle
                VXProcess._drawables.Remove(this);
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Destroying " + gameObject.name, E);
            }
        }
    }
}