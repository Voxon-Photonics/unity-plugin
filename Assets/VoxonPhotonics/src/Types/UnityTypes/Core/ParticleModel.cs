using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    /// <summary>
    ///  The model is independent of the user interface.
    /// It doesn't know if it's being used from a text-based, graphical, or web interface
    /// </summary>
    [Serializable]
    public class ParticleModel
    {
        const float SIZE_MODIFIER = 0.05f;

        GameObject parent;

        ParticleSystem m_particleSystem;
        ParticleSystem.Particle[] m_Particles;
        int n_Particles;

        #region data_manipulation
        public void Update()
        {        }
        #endregion

        #region getters_setters
        public ParticleSystem ParticleSystem
        {
            set {
                this.m_particleSystem = value;
                m_Particles = new ParticleSystem.Particle[m_particleSystem.main.maxParticles];

                // Use worldspace
                var main = m_particleSystem.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
            }
        }
        public int ParticleCount
        {
            get {
                return (m_particleSystem != null) ? m_particleSystem.GetParticles(m_Particles) : 0;
            }
        }

        public float GetParticleSize(int particle_index)
        {
            return m_Particles[particle_index].GetCurrentSize(m_particleSystem) * SIZE_MODIFIER;
        }

        public point3d GetParticle(int particle_index)
        {
            return (VXProcess.Instance.Transform * m_Particles[particle_index].position).toPoint3d();
        }

        public int GetParticleColour(int particle_index)
        {
            return ((m_Particles[particle_index].GetCurrentColor(m_particleSystem)).toInt() & 0xffffff) >> 0;
        }

        public GameObject Parent
        {
            get { return parent; }
            set { parent = value; Update(); }
        }

        Matrix4x4 mat = Matrix4x4.identity;
        public Matrix4x4 GetMatrix(int particle_index)
        {
            mat.SetTRS(m_Particles[particle_index].position, Quaternion.Euler(m_Particles[particle_index].rotation3D), m_Particles[particle_index].GetCurrentSize3D(m_particleSystem)*5);
            return mat;
        }
        #endregion
    }
}
