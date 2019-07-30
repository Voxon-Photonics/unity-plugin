using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class MeshRegister : Singleton<MeshRegister> {
    public ComputeShader cshader_main;
    public int kernelHandle;

	public static bool active = false;

    [SerializeField]
    private Dictionary<String, RegisteredMesh> Register;

    private void OnEnable()
    {
        Init();

        FileStream s = null;
        IFormatter formatter = new BinaryFormatter();
        try
        {
            s = new FileStream(Application.dataPath+ "/StreamingAssets/MeshData.bin", FileMode.Open);
            MeshPack mp = (MeshPack)formatter.Deserialize(s);
            Voxon.MeshData[] md = mp.getData();
            for(int idx = 0; idx < md.Length; idx++)
            {
                Register.Add(md[idx].name, new RegisteredMesh(ref md[idx]));
            }
        }
		catch
		{
			Debug.LogError("Deserialise error on " + (Application.dataPath + "/StreamingAssets/MeshData.bin") + "\nRebuild MeshData file.");
		}
        finally
        {
			if(s != null) s.Close();
        }

		active = true;

	}

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
        Vector3 count = Vector3.zero;
        foreach (var point in mesh.vertices)
        {
            count += point;
        }
        mesh.name = count.x + "-" + count.y + "-" + count.z + ":" + mesh.vertexBufferCount + ":" + mesh.vertexCount + ":" + mesh.subMeshCount + ":" + mesh.triangles.Length;

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
			
			RegisteredMesh rt = Register[mesh.name];
            rt.decrement();

            if (!rt.isactive() && false)
            {
                Register.Remove(mesh.name);
                rt.destroy();
            }
        }
        return false;
    }

    public int Length()
    {
        return Register.Count;
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

	public new void OnApplicationQuit()
	{
		active = false;
		Clear();
		base.OnApplicationQuit();
	}

	private new void OnDestroy()
    {
		active = false;
		Clear();
		base.OnDestroy();
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

    #if UNITY_EDITOR
    public MeshPack PackMeshes()
    {
        Voxon.MeshData[] RMs = new Voxon.MeshData[Register.Count];
        int idx = 0;
        foreach(RegisteredMesh rm in Register.Values)
        {
            RMs[idx] = rm.getMeshData();
            idx++;
        }

        MeshPack packedMeshes = new MeshPack();
        packedMeshes.setData(RMs);

        return packedMeshes;
    }
    #endif
}
