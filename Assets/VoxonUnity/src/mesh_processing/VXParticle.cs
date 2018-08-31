using UnityEngine;
using UnityEngine.Profiling;
using System;
using System.Runtime.InteropServices;



namespace Voxon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VXParticle : MonoBehaviour {

        public struct vector3
        {
            public float x, y, z;
        };

        public struct vector4
        {
            public float x, y, z, filler;
            public vector4(Vector3 values)
            {
                x = values.x;
                y = values.y;
                z = values.z;
                filler = 0f;
            }
        };

        public struct matrix3x4
        {
            public vector4 r, d, f, p;
            public matrix3x4(Vector3 rotations, Vector3 values, Vector3 scale)
            {
                // TODO Scaling doesn't work correctly current (Try setting the scale to 20,8.88, 20) there is a clear offset
                p = new vector4();

                p.x = values.x * scale.x;
                p.y = -values.z * scale.z;
                // Offset the z axis (depth)
                p.z = -values.y * scale.y;

                float cx, sx, cy, sy, cz, sz;
                cx = Mathf.Cos(rotations.x); sx = Mathf.Sin(rotations.x);
                cy = Mathf.Cos(rotations.y); sy = Mathf.Sin(rotations.y);
                cz = Mathf.Cos(rotations.z); sz = Mathf.Sin(rotations.z);
                // float cysz, sxsy, cxsy;

                r = new vector4(new Vector3(cy * cz, -cx * sz + sx * sy * cz, sx * sz + cx * sy * cz));
                d = new vector4(new Vector3(cy * sz, cx * cz + sx * sy * sz, -sx * cz + cx * sy * sz));
                f = new vector4(new Vector3(-sy, sx * cy, cx * cy));

                //d = new vector4(new Vector3(f.y*r.z - f.z*r.y, f.z * r.x - f.x * r.z, f.x * r.y - f.y * r.x));
            }
            public matrix3x4(Matrix4x4 source)
            {
                r.x = source.GetRow(0).x; r.y = source.GetRow(1).x; r.z = source.GetRow(2).x; r.filler = source.GetRow(3).x;
                d.x = source.GetRow(0).y; d.y = source.GetRow(1).y; d.z = source.GetRow(2).y; d.filler = source.GetRow(3).y;
                f.x = source.GetRow(0).z; f.y = source.GetRow(1).z; f.z = source.GetRow(2).z; f.filler = source.GetRow(3).z;
                p.x = source.GetRow(0).w; p.y = source.GetRow(1).w; p.z = source.GetRow(2).w; p.filler = source.GetRow(3).w;
            }
        };

        [DllImport("Poltex_Transform", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void transform_64(DLL.poltex_t[] source, DLL.poltex_t[] dest, ref matrix3x4 rotransmat, int len);

        [DllImport("Poltex_Transform", CallingConvention = CallingConvention.Cdecl)]
        public static extern void transform_mesh(DLL.poltex_t[] source, DLL.poltex_t[] dest, ref matrix3x4 rotransmat, int len);

        private int draw_flags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer

        private ParticleSystem m_particleSystem;
        ParticleSystem.Particle[] m_Particles;
        int n_Particles;

        Matrix4x4 Matrix;

        Renderer rend;
        registered_mesh mesh;

        // DLL Version (Source is mesh.vertices)
        DLL.poltex_t[][] destinations;

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
                rend = GetComponent<Renderer>();
                load_meshes();

                // Particles destination structures
                destinations = new DLL.poltex_t[m_particleSystem.main.maxParticles][];
                for (int idx = m_particleSystem.main.maxParticles - 1; idx >= 0; --idx)
                {
                    destinations[idx] = new DLL.poltex_t[mesh.vertex_count];
                }
            }
            else
            {
                TransformBillboard();
            }

            VXProcess._particles.Add(this);
        }

        // Update is called once per frame
        void Update()
        { }

        public void Draw()
        {
            // TODO : The IsAlive check may cause problems in the future, leaving for now due to safety
            if(!m_particleSystem || !m_particleSystem.IsAlive())
            {
                return;
            }
            if (renderMode == ParticleSystemRenderMode.Mesh)
            {
                if(tag == "Sphere")
                {
                    Draw_Sphere();
                }
                else
                {
                    Draw_Mesh();
                }
                
            }
            else
            {
                Draw_Billboard();
            }
        }

        public void Draw_Billboard()        
        {

            if (VXProcess.Instance._camera.transform.hasChanged)
            {
                TransformBillboard();
            }

            if (m_Particles != null)
            {
                n_Particles = m_particleSystem.GetParticles(m_Particles);

                Vector3 l;

                foreach (var par in m_Particles)
                {
                    float size = par.GetCurrentSize(m_particleSystem) / 25.0f;
                    l = Matrix * par.position;
                    Voxon.DLL.draw_box(l.x - size, -l.y, l.z - size, l.x + size, -l.y, l.z + size, 2, (par.GetCurrentColor(m_particleSystem)).toInt());
                }
            }
        }

        public void Draw_Mesh()
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            Profiler.BeginSample("Particle");

            Profiler.BeginSample("D");
            Matrix4x4 FMatrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * VXProcess.Instance._camera.transform.worldToLocalMatrix;
            Profiler.EndSample();

            matrix3x4 outMat;
            for (int idx = 0; idx < n_Particles; ++idx)
            {
                // Unity Style 

                Profiler.BeginSample("A");

                Matrix.SetTRS(m_Particles[idx].position, Quaternion.Euler(m_Particles[idx].rotation3D), m_Particles[idx].GetCurrentSize3D(m_particleSystem));
                Profiler.EndSample();
                Profiler.BeginSample("B");
                Matrix = FMatrix * Matrix;
                Profiler.EndSample();
                Profiler.BeginSample("C");
                // MeshRegister.Instance.compute_transform_cpu(Matrix, mesh, ref destinations[idx]);
                outMat = new matrix3x4(Matrix);
                transform_mesh(mesh.vertices, destinations[idx], ref outMat, mesh.vertex_count);
                Profiler.EndSample();


                // TODO We don't currently rotate based on the rotation of the capture volume!
                /*
                Vector3 shift = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
                rotmatpos = new matrix3x4(m_Particles[idx].rotation3D, m_Particles[idx].position + shift, m_Particles[idx].GetCurrentSize3D(m_particleSystem));
                transform_mesh(mesh.vertices, destinations[idx], ref rotmatpos, mesh.vertex_count);
                */
                for (int idy = mesh.submesh_count-1; idy >= 0; --idy)
                {
                    DLL.draw_untextured_mesh(destinations[idx], mesh.vertex_count, mesh.indices[idy], mesh.index_counts[idy], draw_flags, (m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt());
                }
            }
            Profiler.EndSample();
        }

        // Draw Sphere
        public void Draw_Sphere()
        {
            n_Particles = m_particleSystem.GetParticles(m_Particles);

            Profiler.BeginSample("Particle");

            float fx, fy, fz;
            for (int idx = 0; idx < n_Particles; ++idx)
            {
                // TODO We don't currently rotate based on the rotation of the capture volume!
                // Vector3 shift = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
                Vector3 f = (m_Particles[idx].position - VXProcess.Instance._camera.transform.position);
                fx = f.x / VXProcess.Instance._camera.transform.lossyScale.x * 2f;
                fz = -f.y / VXProcess.Instance._camera.transform.lossyScale.y * 0.888f;
                fy = -f.z / VXProcess.Instance._camera.transform.lossyScale.z * 2f;
                Voxon.DLL.draw_sphere(fx, fy, fz, m_Particles[idx].GetCurrentSize(m_particleSystem) * 0.05f, 0, ((m_Particles[idx].GetCurrentColor(m_particleSystem)).toInt()&0xffffff)>>0);
            }
            Profiler.EndSample();
        }

        public void TransformBillboard()
        {
            try
            {
                // Set Model View transform
                Matrix = transform.localToWorldMatrix;
                Matrix = VXProcess.Instance._camera.transform.worldToLocalMatrix * Matrix;
                Matrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * Matrix;

            }
            catch (Exception E)
            {
                UnityEngine.Debug.LogError("Error while Building Mesh " + gameObject.name + "\n" + E.Message);
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
    }
}