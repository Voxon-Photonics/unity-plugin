using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Voxon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Particle : MonoBehaviour
    {
        public enum ParticleStyle
        {
            Billboard,
            Box,
            Mesh,
            Sphere
        };

        // Editor View (Won't use after initialisation)
        public ParticleStyle particle_style = ParticleStyle.Billboard;

        // Associated Models
        ParticleModel particleModel;

        // Associated Views
        ParticleView particle;

        // Use this for initialization
        void Start()
        {
            ParticleSystem ps;
            try
            {
                ps = GetComponent<ParticleSystem>();
                particleModel = new ParticleModel();
                particleModel.ParticleSystem = ps;
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("(" + name + ") Failed to load suitable Line", E);
                Destroy(this);
            }

            switch(particle_style)
            {
                case ParticleStyle.Billboard:
                    particle = new ParticleViewBillBoard(particleModel);
                    break;
                case ParticleStyle.Box:
                    particle = new ParticleViewBox(particleModel);
                    break;
                case ParticleStyle.Sphere:
                    particle = new ParticleViewSphere(particleModel);
                    break;
                case ParticleStyle.Mesh:
                    particle = new ParticleViewMesh(particleModel, GetComponent<ParticleSystemRenderer>().mesh);
                    break;
                default:
                    break;
            }

            if(particle == null || particleModel == null)
            {
                Debug.LogError("Particle? " + (particle != null) + " ParticleModel? " + (particleModel != null));
                Destroy(this);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}