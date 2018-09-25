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
        mesh.name = "" + mesh.vertexBufferCount + ":" + mesh.vertexCount + ":" + mesh.subMeshCount + ":" + mesh.triangles.Length;

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

            // TODO decide when to drop mesh (disable mesh dropping for now)
            /*
            if (!rt.isactive() && false)
            {
                Register.Remove((mesh.name));
                rt.destroy();

                return true;
            }
            */
        }
        return false;
    }

    public void Clear()
    {
        if (Register == null)
            return;

        while (Register.Count > 0)
        {
            RemoveRegister(Register.ElementAt(0).Key);
        }
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        Clear();
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
}
