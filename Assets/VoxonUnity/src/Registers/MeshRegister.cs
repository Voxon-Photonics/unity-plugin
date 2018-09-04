using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MeshRegister : Singleton<MeshRegister> {

    public ComputeShader cshader_main;
    public int kernelHandle;

    private Dictionary<String, RegisteredMesh> Register;

    void Init()
    {
        Register = new Dictionary<String, RegisteredMesh>();

        if (!Resources.Load("VCS"))
            Debug.Log("Failed to load VCS");

        cshader_main = (ComputeShader)Resources.Load("VCS");
        kernelHandle = cshader_main.FindKernel("CSMain");
    }

    public RegisteredMesh get_registed_mesh(ref Mesh mesh)
    {
        if(mesh.name == "")
        {
            mesh.name = "" + mesh.vertexBufferCount + ":" + mesh.vertexCount + ":" + mesh.subMeshCount + ":" + mesh.triangles.Length;
            Debug.Log("No Name, New name: " + mesh.name);
        }        

        if (Register == null)
        {
            Init();
        }

        if (Register.ContainsKey(mesh.name))
        {
            RegisteredMesh rm = Register[(mesh.name)];
            rm.increment();

            return rm;
        }
        else
        {
            RegisteredMesh rm = new RegisteredMesh(ref mesh);

            Register.Add(mesh.name, rm);

            return rm;
        }
    }

    public bool drop_mesh(ref Mesh mesh)
    {
        if (Register != null && Register.ContainsKey(mesh.name))
        {
            RegisteredMesh rt = Register[(mesh.name)];
            rt.decrement();

            if (!rt.isactive() && false)
            {
                Register.Remove((mesh.name));
                rt.destroy();

                return true;
            }
        }
        return false;
    }

    /// <summary>  
    ///  Compute Shader call. Set up Kernel, define tranform values and dispatches GPU threads
    ///  Currently only sends thin batches; should see to increase this in future.
    ///  </summary>

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
        else
        {
            RegisteredMesh rt = Register[name];
            Register.Remove(name);
            rt.destroy();
        }
    }

    public void compute_transform(String name, Matrix4x4 Transform, ref Voxon.DLL.poltex_t[] vt)
    {
        if (Register.ContainsKey(name))
        {
            // Register[name].compute_transform(Transform, ref vt);
        }
    }

    public void compute_transform_cpu(String name, Matrix4x4 Transform, ref Voxon.DLL.poltex_t[] vt)
    {
        if (Register.ContainsKey(name))
        {
            Register[name].compute_transform_cpu(Transform, ref vt);
        }
    }

    public void compute_transform_anim(String name, Matrix4x4 Transform, ref Voxon.DLL.poltex_t[] vt, ref Mesh mesh)
    {
        if (Register.ContainsKey(name))
        {
            Register[name].compute_transform(Transform, ref vt, ref mesh);
        }
    }

}
