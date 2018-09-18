using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class RegisteredMesh {
    // Mesh Data
    public Voxon.DLL.poltex_t[] vertices;
    public string name;
    public int vertex_count;        // Number of vertices
    public int submesh_count;      // Count of Submeshes part of mesh
    public int[][] indices;
    public int[] index_counts;

    // Shader operations
    ComputeBuffer cbufferI_vertices;
    ComputeBuffer cbufferI_uvs;
    ComputeBuffer cbufferO_poltex;
    public int counter = 1;     // If instantiated we expect atleast 1, so set that as default value

    bool loaded;

    // Use this for initialization
    public RegisteredMesh(ref Mesh mesh)
    {
        try
        {
            name = mesh.name;

            // Mesh Divisions
            submesh_count = mesh.subMeshCount;

            // Vertices
            vertex_count = mesh.vertexCount;
            vertices = new Voxon.DLL.poltex_t[vertex_count];
            load_poltex(ref mesh);
            // UVs
            List<Vector2> tmp_uvs = new List<Vector2>();
            mesh.GetUVs(0, tmp_uvs);

            // Mesh may not have uvs; default to 0,0 to ensure we have values
            if (tmp_uvs.Count < mesh.vertexCount)
            {
                tmp_uvs.AddRange(Enumerable.Repeat(Vector2.zero, mesh.vertexCount - tmp_uvs.Count));
            }

            for (int idx = vertex_count - 1; idx >= 0; --idx)
            {
                vertices[idx].u = tmp_uvs[idx].x;
                vertices[idx].v = tmp_uvs[idx].y;
            }

            // Indexes
            indices = new int[submesh_count][];
            index_counts = new int[submesh_count];

            // Triangles
            rearrange_indices(ref mesh);

            // Set up output buffer; the assigned data array will change per instance
            cbufferO_poltex = new ComputeBuffer(vertex_count, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);

            cbufferI_uvs = new ComputeBuffer(vertex_count, sizeof(float) * 2, ComputeBufferType.Default);
            cbufferI_uvs.SetData(tmp_uvs.ToArray());
            cbufferI_vertices = new ComputeBuffer(vertex_count, sizeof(float) * 3, ComputeBufferType.Default);
            cbufferI_vertices.SetData(mesh.vertices);

        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error building Mesh " + name, E);
        }
    }

    public void increment()
    {
        counter++;
    }

    public void decrement()
    {
        counter--;
    }

    public bool isactive()
    {
        return counter > 0;
    }

    public static Voxon.DLL.poltex_t build_poltex(Vector3 pos, Vector2 uv, int col)
    {
        Voxon.DLL.poltex_t _T = new Voxon.DLL.poltex_t();
        _T.x = pos.x;
        _T.y = pos.y;
        _T.z = pos.z;
        _T.u = uv.x;
        _T.v = uv.y;
        _T.col = col;
        return _T;
    }

    public void update_baked_mesh(SkinnedMeshRenderer sm_rend, ref Mesh mesh)
    {
        sm_rend.BakeMesh(mesh);
    }

    /** Privates **/
    private void load_poltex(ref Mesh mesh)
    {
        // Set Source Vertices
        int uv_length = mesh.uv.Length;
        int colour = mesh.colors.Length > 0 ? mesh.colors[0].toInt() : 0xFFFFFF;

        Vector3[] verts = mesh.vertices;
        Vector2[] uvs = mesh.uv;

        for (int idx = vertex_count - 1; idx >= 0; --idx)
        {
            vertices[idx] = build_poltex(verts[idx], uv_length == 0 ? Vector2.zero : uvs[idx], colour);
        }

    }

    private void rearrange_indices(ref Mesh mesh)
    {
        try
        {
            for (int submesh = 0; submesh < submesh_count; submesh++)
            {

                /* Triangles are 3 idx and our array requires -1 delimiting, 
                /  So we need to make room for all tris (count) + a -1 at the end of each (count / 3)
                */

                // Note GetTriangles.Length takes ages, save as int and use that instead
                int _indices = mesh.GetTriangles(submesh).Length;
                index_counts[submesh] = _indices + (_indices / 3);


                indices[submesh] = new int[index_counts[submesh]]; // Number of Poly Indices

                // Set up indices

                int[] Tri_me = mesh.GetTriangles(submesh);

                int out_idx = 0;
                for (int i = 0; i < _indices; i += 3, out_idx += 4)
                {
                    // Copy internal array to output array
                    indices[submesh][0 + out_idx] = Tri_me[i + 0];
                    indices[submesh][1 + out_idx] = Tri_me[i + 1];
                    indices[submesh][2 + out_idx] = Tri_me[i + 2];

                    // flag end of triangle
                    indices[submesh][3 + out_idx] = -1;
                }

                /*
                Debug.Log(indices[submesh].Length);
                for (int idx = 0; idx < indices[submesh].Length; idx += 4)
                {
                    Debug.Log(idx + "\t" + indices[submesh][0 + idx] + " : " + indices[submesh][1 + idx] + " : " + indices[submesh][2 + idx] + " : " + indices[submesh][3 + idx]);
                }
                */
            }
        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error while Translating Triangles " + name, E);
        }
    }

    /// <summary>  
    ///  Compute Shader call. Set up Kernel, define tranform values and dispatches GPU threads
    ///  Currently only sends thin batches; should see to increase this in future.
    ///  </summary>
    public void compute_transform_gpu(Matrix4x4 Transform, ref Voxon.DLL.poltex_t[] vt, ref Mesh mesh)
    {
        try
        {
            // Need to be set for each draw update
            MeshRegister.Instance.cshader_main.SetMatrix("_transform", Transform);

            cbufferI_vertices.SetData(mesh.vertices);

            MeshRegister.Instance.cshader_main.SetBuffer(MeshRegister.Instance.kernelHandle, "in_vertices", cbufferI_vertices);
            MeshRegister.Instance.cshader_main.SetBuffer(MeshRegister.Instance.kernelHandle, "in_uvs", cbufferI_uvs);
            MeshRegister.Instance.cshader_main.SetBuffer(MeshRegister.Instance.kernelHandle, "output", cbufferO_poltex);

            // Slight Magic Number; Aiming to create 256 int blocks
            int blocks = (vertex_count + 256 - 1) / 256;

            // cshader_main.Dispatch(kernelHandle, blocks, subblocks, 1);
            MeshRegister.Instance.cshader_main.Dispatch(MeshRegister.Instance.kernelHandle, blocks, 1, 1);
            
            cbufferO_poltex.GetData(vt);

        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error while Computing Transform " + name, E);
        }
    }

    public void compute_transform_cpu(Matrix4x4 component, ref Voxon.DLL.poltex_t[] vt)
    {
        Vector4 in_v;
        for (int idx = vertices.Length - 1; idx >= 0; --idx)
        {
            in_v = new Vector4(vertices[idx].x, vertices[idx].y, vertices[idx].z, 1.0f);

            in_v = component * in_v;

            vt[idx].x = in_v.x;
            vt[idx].y = -in_v.z;
            vt[idx].z = -in_v.y;
            vt[idx].u = vertices[idx].u;
            vt[idx].v = vertices[idx].v;
        }
    }

    public void compute_transform_anim(Matrix4x4 component, ref Voxon.DLL.poltex_t[] vt, ref Mesh mesh)
    {

        // Vector3 pos = component.transform.position;object_transform.MultiplyPoint3x4(component.transform.position)
        // Matrix4x4 object_transform = VXProcess.Instance._camera.transform.worldToLocalMatrix;

        Vector4 in_v;
        Vector3 [] verts = mesh.vertices;
        for (int idx = verts.Length - 1; idx >= 0; --idx)
        {
            in_v = new Vector4(verts[idx].x, verts[idx].y, verts[idx].z, 1.0f);

            in_v = component * in_v;

            vt[idx].x = in_v.x;
            vt[idx].y = -in_v.z;
            vt[idx].z = -in_v.y;
            vt[idx].u = vertices[idx].u;
            vt[idx].v = vertices[idx].v;
        }
    }
    public void destroy()
    {
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
    }

}
