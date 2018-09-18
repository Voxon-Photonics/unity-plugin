using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace Voxon
{
    public enum flags
    {
        dots = 0,
        lines = 1,
        surfaces = 2,
        solid = 3
    };

    public class VXComponent : MonoBehaviour
    {
        private Renderer rend;
        private SkinnedMeshRenderer sm_rend;

        // Mesh Structure Structure
        private Mesh Umesh;         // Objects mesh
        private Material[] Umaterials; // Object's Materials

        // Mesh Data
        private RegisteredMesh mesh;
        private Voxon.DLL.poltex_t[] vt;   // List of vertices

        private int submesh_n = 0;      // Count of Submeshes part of mesh

        public flags _flags = flags.surfaces;
        private int draw_flags = 2 | 1 << 3; // 2 - Fill, and Draw from Texture buffer
        private int[] cols;

        // Texture
        Voxon.DLL.tiletype[] textures;

        private Matrix4x4 _transform;

        // Use this for initialization
        void Start()
        {
            draw_flags = (int)_flags | 1 << 3;
            try
            {
                try
                {
                    rend = GetComponent<Renderer>();
                    rend.enabled = true;
                }
                catch (Exception E)
                {
                    ExceptionHandler.Except("(" + name + ") Failed to load suitable Renderer", E);
                }

                sm_rend = GetComponent<SkinnedMeshRenderer>();
                if (sm_rend)
                {
                    Umesh = new Mesh();

                    sm_rend.BakeMesh(Umesh);
                    Umaterials = sm_rend.materials;

                    sm_rend.updateWhenOffscreen = true;
                }
                else
                {
                    Umesh = GetComponent<MeshFilter>().sharedMesh;
                    Umaterials = GetComponent<MeshRenderer>().materials;
                }


                if (Umesh == null)
                {
                    ExceptionHandler.Except("(" + name + ") Mesh: FAILED TO LOAD", new NullReferenceException());
                }

                if (Umaterials.Length != Umesh.subMeshCount)
                {
                    ExceptionHandler.Except("ERROR: " + name + " has mismatching materials and submesh count. These need to be equal! Submesh past material count will be assigned first material", new Exception());
                }

                // Load Mesh
                load_meshes();

                // Load textures
                load_textures();

                VXProcess._components.Add(this);
            }
            catch (Exception E)
            {

                ExceptionHandler.Except("Error while building " + gameObject.name, E);
            }

        }

        public void set_flag(int flag)
        {
            _flags = (flags)flag;
            draw_flags = flag | 1 << 3;
        }

        // Use destroy to free gpu data
        void OnDestroy()
        {
            try
            {
                // Remove ourselves from Draw cycle
                VXProcess._components.Remove(this);

                // Free Mesh
                if(Umesh)
                    MeshRegister.Instance.drop_mesh(ref Umesh);

                for (int submesh = 0; submesh < submesh_n; submesh++)
                {
                    if (Umaterials[submesh].mainTexture)
                    {
                        TextureRegister.Instance.drop_tile(ref Umaterials[submesh]);
                    }
                }
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Destroying " + gameObject.name, E);
            }
        }

        /// <summary>  
        ///  Draw the drawable mesh; Uses Capture Volume's transform to determine if play space has changed
        ///  Animated meshes are set to redraw every frame while statics only redrawn on them or the volume
        ///  changing transform.
        ///  </summary>
        public void DrawMesh()
        {
            try
            {
                if (!gameObject.activeSelf || tag == "VoxieHide")
                    return;

                if (sm_rend)
                {
                    mesh.update_baked_mesh(sm_rend, ref Umesh);
                }

                
                if (sm_rend || VXProcess.Instance._camera.transform.hasChanged || transform.hasChanged)
                {
                    BuildMesh();
                }


                for (int idx = 0; idx < mesh.submesh_count; idx++)
                {
                    if (Umaterials[idx].mainTexture)
                    {
                        Voxon.DLL.draw_textured_mesh(ref textures[idx], vt, mesh.vertex_count, mesh.indices[idx], mesh.index_counts[idx], draw_flags);
                    }
                    else
                    {
                        Voxon.DLL.draw_untextured_mesh(vt, mesh.vertex_count, mesh.indices[idx], mesh.index_counts[idx], draw_flags, rend.materials[idx].color.toInt());
                    }
                }
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Drawing " + gameObject.name, E);
            }
        }

        /// <summary>  
        ///  Generates relevant transform for mesh type against capture volume transform
        ///  Passes to the Compute Shader for processing
        ///  </summary>
        private void BuildMesh()
        {
            try
            {
                // Set Model View transform
                Matrix4x4 Matrix;
                Matrix = transform.localToWorldMatrix;

                Matrix = VXProcess.Instance._camera.transform.worldToLocalMatrix * Matrix;
                Matrix = Matrix4x4.Scale(new Vector3(2.0f, 0.8f, 2.0f)) * Matrix;

                if (sm_rend)
                {
                    MeshRegister.Instance.compute_transform_anim(mesh.name, Matrix, ref vt, ref Umesh);
                }
                else
                {
                    MeshRegister.Instance.compute_transform_cpu(mesh.name, Matrix, ref vt);
                }

                transform.hasChanged = false;
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Building Mesh " + gameObject.name, E);
            }
        }

        private void load_meshes()
        {
            try
            {
                mesh = MeshRegister.Instance.get_registed_mesh(ref Umesh);
                vt = new DLL.poltex_t[mesh.vertex_count];

                BuildMesh();
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Loading Mesh: " + gameObject.name, E);
            }
        }

        private void load_textures()
        {
            try
            {
                textures = new Voxon.DLL.tiletype[mesh.submesh_count];
                for (int submesh = 0; submesh < mesh.submesh_count; submesh++)
                {
                    if (Umaterials[submesh].mainTexture)
                    {
                        textures[submesh] = TextureRegister.Instance.get_tile(ref Umaterials[submesh]);
                    }
                }
            }
            catch (Exception E)
            {
                ExceptionHandler.Except("Error while Loading Textures: " + gameObject.name, E);
            }
        }

        
    }
}
