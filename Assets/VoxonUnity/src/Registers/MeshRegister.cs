using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public struct registered_mesh
{
    // Mesh Data
    public Voxon.DLL.poltex_t[] vertices;
    public int vertex_count;        // Number of vertices
    public int submesh_count;      // Count of Submeshes part of mesh
    public int[][] indices;
    public int[] index_counts;
    // Shader operations
    public ComputeBuffer cbufferI_vertices;
    public ComputeBuffer cbufferI_uvs;
    public ComputeBuffer cbufferO_poltex;
    public int counter;
}

public class MeshRegister : Singleton<MeshRegister> {

    private ComputeShader cshader_main;
    private int kernelHandle;

    private Dictionary<String, registered_mesh> Register;

    // Use this for initialization
    void Start() {
    }

    void Init()
    {
        Register = new Dictionary<String, registered_mesh>();

        if (!Resources.Load("VCS"))
            Debug.Log("Failed to load VCS");

        cshader_main = (ComputeShader)Resources.Load("VCS");
        kernelHandle = cshader_main.FindKernel("CSMain");
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

    public registered_mesh get_registed_mesh(ref Mesh mesh)
    {
        if (Register == null)
        {
            Init();
        }

        if (Register.ContainsKey(mesh.name))
        {
            registered_mesh rm = Register[(mesh.name)];
            rm.counter++;
            Register[(mesh.name)] = rm;


            return rm;
        }
        else
        {
            registered_mesh rm = build_mesh(ref mesh, true);
            rearrange_indices(ref rm, ref mesh);
            rm.counter = 1;

            Register.Add(mesh.name, rm);

            return rm;
        }
    }

    public registered_mesh build_mesh(ref Mesh mesh, bool static_mesh)
    {
        registered_mesh rm = new registered_mesh();
        try
        {
            // Mesh Divisions
            rm.submesh_count = mesh.subMeshCount;

            // Vertices
            rm.vertex_count = mesh.vertexCount;
            rm.vertices = new Voxon.DLL.poltex_t[rm.vertex_count];

            load_poltex(ref rm, ref mesh);

            // UVs
            List<Vector2> tmp_uvs = new List<Vector2>();
            mesh.GetUVs(0, tmp_uvs);

            // Mesh may not have uvs; default to 0,0 to ensure we have values
            if (tmp_uvs.Count < mesh.vertexCount)
            {
                tmp_uvs.AddRange(Enumerable.Repeat(Vector2.zero, mesh.vertexCount - tmp_uvs.Count));
            }

            for(int idx = rm.vertices.Length -1; idx >= 0; --idx)
            {
                rm.vertices[idx].u = tmp_uvs[idx].x;
                rm.vertices[idx].v = tmp_uvs[idx].y;
            }

            // Indexes
            rm.indices = new int[rm.submesh_count][];
            rm.index_counts = new int[rm.submesh_count];

            // TODO Disabling to test if not needed
            
            // Set up output buffer; the assigned data array will change per instance
            rm.cbufferO_poltex = new ComputeBuffer(rm.vertex_count, sizeof(float) * 5 + sizeof(int), ComputeBufferType.Default);
            rm.cbufferI_uvs = new ComputeBuffer(rm.vertex_count, sizeof(float) * 2, ComputeBufferType.Default);
            rm.cbufferI_uvs.SetData(tmp_uvs.ToArray());

            // If our mesh is static, we will set the values now
            rm.cbufferI_vertices = new ComputeBuffer(rm.vertex_count, sizeof(float) * 3, ComputeBufferType.Default);
            rm.cbufferI_vertices.SetData(mesh.vertices);

        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error building Mesh" , E);
        }
        return rm;
    }

    public static void load_poltex(ref registered_mesh rm, ref Mesh mesh)
    {
        // Set Source Vertices
        for (int idx = rm.vertex_count - 1; idx >= 0; --idx)
        {
            rm.vertices[idx] = build_poltex(mesh.vertices[idx], mesh.uv.Length == 0 ? Vector2.zero : mesh.uv[idx], mesh.colors.Length > 0 ? mesh.colors[0].toInt() : 0xFFFFFF);
        }
    }

    private void rearrange_indices(ref registered_mesh rm, ref Mesh mesh)
    {
        try
        {
            for (int submesh = 0; submesh < rm.submesh_count; submesh++)
            {

                /* Triangles are 3 idx and our array requires -1 delimiting, 
                /  So we need to make room for all tris (count) + a -1 at the end of each (count / 3)
                */

                // Note GetTriangles.Length takes ages, save as int and use that instead
                int indices = mesh.GetTriangles(submesh).Length;
                rm.index_counts[submesh] = indices + (indices / 3);

                rm.indices[submesh] = new int[rm.index_counts[submesh]]; // Number of Poly Indices

                // Set up indices

                int[] Tri_me = mesh.GetTriangles(submesh);

                int out_idx = 0;
                for (int i = 0; i < indices; i += 3)
                {
                    // Copy internal array to output array
                    rm.indices[submesh][0 + out_idx] = Tri_me[i + 0];
                    rm.indices[submesh][1 + out_idx] = Tri_me[i + 1];
                    rm.indices[submesh][2 + out_idx] = Tri_me[i + 2];

                    // flag end of triangle
                    rm.indices[submesh][3 + out_idx] = -1;

                    out_idx += 4;
                }
            }
        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error while Translating Triangles " + gameObject.name, E);
        }
    }


    public bool drop_mesh(ref Mesh mesh)
    {
        if (Register != null && Register.ContainsKey(mesh.name))
        {
            registered_mesh rt = Register[(mesh.name)];
            rt.counter--;

            if (rt.counter <= 0)
            {
                Register.Remove(mesh.name);
                if (rt.cbufferI_uvs != null)
                {
                    rt.cbufferI_uvs.Release();
                }
                if (rt.cbufferI_vertices != null)
                {
                    rt.cbufferI_vertices.Release();
                }
                if (rt.cbufferO_poltex != null)
                {
                    rt.cbufferO_poltex.Release();
                }

                return true;
            }
            else
            {
                Register[mesh.name] = rt;
            }
        }
        return false;
    }

    /// <summary>  
    ///  Compute Shader call. Set up Kernel, define tranform values and dispatches GPU threads
    ///  Currently only sends thin batches; should see to increase this in future.
    ///  </summary>
    public void compute_transform(Matrix4x4 Transform, registered_mesh rm, ref Voxon.DLL.poltex_t[] vt)
    {
        try
        {
            // Need to be set for each draw update
            cshader_main.SetMatrix("_transform", Transform);

            cshader_main.SetBuffer(kernelHandle, "in_vertices", rm.cbufferI_vertices);
            cshader_main.SetBuffer(kernelHandle, "in_uvs", rm.cbufferI_uvs);
            cshader_main.SetBuffer(kernelHandle, "output", rm.cbufferO_poltex);


            // Slight Magic Number; Aiming to create 256 int blocks
            int blocks = (rm.vertex_count + 256 - 1) / 256;


            cshader_main.Dispatch(kernelHandle, blocks, 1, 1);


            rm.cbufferO_poltex.GetData(vt);

        }
        catch (Exception E)
        {
            Voxon.ExceptionHandler.Except("Error while Computing Transform " + gameObject.name, E);
        }
    }

    public void compute_transform_cpu(Matrix4x4 component, registered_mesh rm, ref Voxon.DLL.poltex_t[] vt)
    {

        // Vector3 pos = component.transform.position;object_transform.MultiplyPoint3x4(component.transform.position)
        // Matrix4x4 object_transform = VXProcess.Instance._camera.transform.worldToLocalMatrix;
        Vector4 in_v;
        for (int idx = rm.vertices.Length - 1; idx >=0; --idx)
        {            
            in_v = new Vector4(rm.vertices[idx].x, rm.vertices[idx].y, rm.vertices[idx].z, 1.0f);

            in_v = component*in_v;

            vt[idx].x = in_v.x;
            vt[idx].y = -in_v.z;
            vt[idx].z = -in_v.y;
            vt[idx].u = rm.vertices[idx].u;
            vt[idx].v = rm.vertices[idx].v;
        }   
    }


    private new void OnDestroy()
    {
        base.OnDestroy();

        if (Register == null)
            return;

        while(Register.Count > 0)
        {
            RemoveRegister(Register.ElementAt(0).Key);
        }
    }

    private void RemoveRegister(string name)
    {
        if (!Register.ContainsKey(name))
        {
            return;
        }

        registered_mesh rt = Register[name];

        Register.Remove(name);

        if (rt.cbufferI_uvs != null)
        {
            rt.cbufferI_uvs.Release();
        }
        if (rt.cbufferI_vertices != null)
        {
            rt.cbufferI_vertices.Release();
        }
        if (rt.cbufferO_poltex != null)
        {
            rt.cbufferO_poltex.Release();
        }
    }
}
