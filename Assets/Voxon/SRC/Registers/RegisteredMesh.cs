﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class RegisteredMesh {
        // Mesh Data
        public poltex[] vertices;
        public string name;
        [FormerlySerializedAs("vertex_count")] public int vertexCount;        // Number of vertices
        [FormerlySerializedAs("submesh_count")] public int submeshCount;      // Count of Submeshes part of mesh
        public int[][] indices;
        [FormerlySerializedAs("index_counts")] public int[] indexCounts;

        // Shader operations
        ComputeBuffer _cbufferIVertices;
        ComputeBuffer _cbufferIUvs;
        ComputeBuffer _cbufferOPoltex;
        public int counter = 1;     // If instantiated we expect atleast 1, so set that as default value

        bool _loaded;

        // Use this for initialization
        public RegisteredMesh(ref Mesh mesh)
        {
            try
            {
                // Debug.Log("Processing Mesh Data");
                name = mesh.name;

                // Mesh Divisions
                submeshCount = mesh.subMeshCount;

                // Vertices
                vertexCount = mesh.vertexCount;

                vertices = new poltex[vertexCount];
                load_poltex(ref mesh);

                // UVs
                var tmpUvs = new List<Vector2>();
                mesh.GetUVs(0, tmpUvs);

                // Mesh may not have uvs; default to 0,0 to ensure we have values
                if (tmpUvs.Count < mesh.vertexCount)
                {
                    tmpUvs.AddRange(Enumerable.Repeat(Vector2.zero, mesh.vertexCount - tmpUvs.Count));
                }

                for (int idx = vertexCount - 1; idx >= 0; --idx)
                {
                    vertices[idx].u = tmpUvs[idx].x;
                    vertices[idx].v = tmpUvs[idx].y;
                }

                // Indexes
                indices = new int[submeshCount][];
                indexCounts = new int[submeshCount];

                // Triangles
                rearrange_indices(ref mesh);

                // Set up output buffer; the assigned data array will change per instance
                _cbufferOPoltex = new ComputeBuffer(vertexCount, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);

                _cbufferIUvs = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Default);
                _cbufferIUvs.SetData(tmpUvs.ToArray());
                _cbufferIVertices = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Default);
                _cbufferIVertices.SetData(mesh.vertices);

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error building Mesh {name}", e);
            }
        }

        public RegisteredMesh(ref MeshData meshData)
        {
            try
            {
                name = meshData.name;

                // Mesh Divisions
                submeshCount = meshData.submeshCount;

                // Vertices
                vertexCount = meshData.vertexCount;
                vertices = meshData.vertices;

                // Indexes
                indices = meshData.indices;
                indexCounts = meshData.indexCounts;

                var tmpUvs = new List<Vector2>();
                var tmpVerts = new List<Vector3>();
                foreach (poltex vert in meshData.vertices)
                {
                    tmpUvs.Add(new Vector2(vert.u, vert.v));
                    tmpVerts.Add(new Vector3(vert.x, vert.y, vert.z));
                }

                // Set up output buffer; the assigned data array will change per instance
                _cbufferOPoltex = new ComputeBuffer(vertexCount, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);

                _cbufferIUvs = new ComputeBuffer(vertexCount, sizeof(float) * 2, ComputeBufferType.Default);
                _cbufferIUvs.SetData(tmpUvs.ToArray());

                _cbufferIVertices = new ComputeBuffer(vertexCount, sizeof(float) * 3, ComputeBufferType.Default);
                _cbufferIVertices.SetData(tmpVerts.ToArray());

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error building Mesh {name}", e);
            }
        }
        public void Increment()
        {
            counter++;
        }

        public void Decrement()
        {
            counter--;
        }

        public bool Isactive()
        {
            return counter > 0;
        }

        public static poltex build_poltex(Vector3 pos, Vector2 uv, int col)
        {
            var t = new poltex
            {
                x = pos.x,
                y = pos.y,
                z = pos.z,
                u = uv.x,
                v = uv.y,
                col = col
            };
            return t;
        }

        public static void update_baked_mesh(SkinnedMeshRenderer smRend, ref Mesh mesh)
        {
            smRend.BakeMesh(mesh);
        }

        /** Privates **/
        private void load_poltex(ref Mesh mesh)
        {
            // Set Source Vertices
            int uvLength = mesh.uv.Length;

            int colour = mesh.colors.Length > 0 ? mesh.colors[0].ToInt() : 0xFFFFFF;

            Vector3[] verts = mesh.vertices;
            Vector2[] uvs = mesh.uv;

            for (int idx = vertexCount - 1; idx >= 0; --idx)
            {
                vertices[idx] = build_poltex(verts[idx], uvLength == 0 ? Vector2.zero : uvs[idx], colour);
            }

        }

        private void rearrange_indices(ref Mesh mesh)
        {
            /* Triangles are 3 idx and our array requires -1 delimiting, 
        /  So we need to make room for all tris (count) + a -1 at the end of each (count / 3)
        */
            try
            {
                for (var submesh = 0; submesh < submeshCount; submesh++)
                {
                    int indicesCount = mesh.GetTriangles(submesh).Length;
                    indexCounts[submesh] = indicesCount + (indicesCount / 3);

                    indices[submesh] = new int[indexCounts[submesh]]; // Number of Poly Indices

                    // Set up indices
                    int[] triMe = mesh.GetTriangles(submesh);

                    var outIdx = 0;
                    for (var i = 0; i < indicesCount; i += 3, outIdx += 4)
                    {
                        // Copy internal array to output array
                        indices[submesh][0 + outIdx] = triMe[i + 0];
                        indices[submesh][1 + outIdx] = triMe[i + 1];
                        indices[submesh][2 + outIdx] = triMe[i + 2];

                        // flag end of triangle
                        indices[submesh][3 + outIdx] = -1;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Translating Triangles {name}", e);
            }
        }

        /// <summary>  
        ///  Compute Shader call. Set up Kernel, define tranform values and dispatches GPU threads
        ///  Currently only sends thin batches; should see to increase this in future.
        ///  </summary>
        public void compute_transform_gpu(Matrix4x4 transform, ref poltex[] vt, ref Mesh mesh)
        {
        
            try
            {
                // Need to be set for each draw update
                MeshRegister.Instance.cshaderMain.SetMatrix("_transform", transform);

                _cbufferIVertices.SetData(mesh.vertices);

                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "in_vertices", _cbufferIVertices);
                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "in_uvs", _cbufferIUvs);
                MeshRegister.Instance.cshaderMain.SetBuffer(MeshRegister.Instance.kernelHandle, "output", _cbufferOPoltex);

                // Slight Magic Number; Aiming to create 256 int blocks
                int blocks = (vertexCount + 256 - 1) / 256;

                // cshader_main.Dispatch(kernelHandle, blocks, subblocks, 1);
                MeshRegister.Instance.cshaderMain.Dispatch(MeshRegister.Instance.kernelHandle, blocks, 1, 1);
            
                _cbufferOPoltex.GetData(vt);

            }
            catch (Exception e)
            {
                ExceptionHandler.Except($"Error while Computing Transform {name}", e);
            }
        }

        public void compute_transform_cpu(Matrix4x4 component, ref poltex[] vt)
        {
            Vector4 inV = Vector4.one;

            for (int idx = vertices.Length - 1; idx >= 0; --idx)
            {
                inV.x = vertices[idx].x;
                inV.y = vertices[idx].y;
                inV.z = vertices[idx].z;
            
                inV = component * inV;
            
                vt[idx].x = inV.x;
                vt[idx].y = -inV.z;
                vt[idx].z = -inV.y;
                vt[idx].u = vertices[idx].u;
                vt[idx].v = vertices[idx].v;
            
            }

        
        }

        public void Destroy()
        {
#if UNITY_2017
		if (cbufferI_uvs != null)
		{
			cbufferI_uvs.Release();
		}
		if (cbufferI_vertices != null)
		{
			cbufferI_vertices.Release();
		}
		if (cbufferO_poltex != null)
		{
			cbufferO_poltex.Release();
		}
#else
            if (_cbufferIUvs.IsValid())
            {
                _cbufferIUvs.Release();
                _cbufferIUvs.Dispose();
                _cbufferIUvs = null;
            }
            if (_cbufferIVertices.IsValid())
            {
                _cbufferIVertices.Release();
                _cbufferIVertices.Dispose();
                _cbufferIVertices = null;
            }
            if (_cbufferOPoltex.IsValid())
            {
                _cbufferOPoltex.Release();
                _cbufferOPoltex.Dispose();
                _cbufferOPoltex = null;
            }
#endif
        }

#if UNITY_EDITOR
        public MeshData GetMeshData()
        {
            var meshData = new MeshData
            {
                indexCounts = indexCounts,
                indices = indices,
                name = name,
                submeshCount = submeshCount,
                vertexCount = vertexCount,
                vertices = vertices
            };

            return meshData;
        }
#endif
    }
}
