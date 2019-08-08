using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class MeshRegister : Singleton<MeshRegister> {
        [FormerlySerializedAs("cshader_main")] public ComputeShader cshaderMain;
        public int kernelHandle;

        public static bool Active;

        [FormerlySerializedAs("Register")] [SerializeField]
        private Dictionary<string, RegisteredMesh> register;

        private void OnEnable()
        {
            Init();

            IFormatter formatter = new BinaryFormatter();

            using (var s = new FileStream(Application.dataPath+ "/StreamingAssets/MeshData.bin", FileMode.Open))
            {
                try
                {
                    var mp = (MeshPack)formatter.Deserialize(s);
                    MeshData[] md = mp.GetData();
                    for(var idx = 0; idx < md.Length; idx++)
                    {
                        register.Add(md[idx].name, new RegisteredMesh(ref md[idx]));
                    }
                }
                catch
                {
                    string err = $"{Application.dataPath}/StreamingAssets/MeshData.bin";
                    Debug.LogError($"Deserialise error on {err}\nRebuild MeshData file.");
                }
            }

            Active = true;

        }

        private void Init()
        {
            register = new Dictionary<string, RegisteredMesh>();

            if (!Resources.Load("VCS"))
                Debug.Log("Failed to load VCS");

            cshaderMain = (ComputeShader)Resources.Load("VCS");
            kernelHandle = cshaderMain.FindKernel("CSMain");
        }

        public RegisteredMesh get_registed_mesh(ref Mesh mesh)
        {
            Vector3 count = mesh.vertices.Aggregate(Vector3.zero, (current, point) => current + point);
        
            mesh.name =
                $"{count.x}-{count.y}-{count.z}:{mesh.vertexBufferCount}:{mesh.vertexCount}:{mesh.subMeshCount}:{mesh.triangles.Length}";

            if (register == null)
            {
                Init();
            }

            if (register.ContainsKey(mesh.name))
            {
                RegisteredMesh rm = register[(mesh.name)];
                rm.Increment();

                return rm;
            }
            else
            {
                var rm = new RegisteredMesh(ref mesh);

                register.Add(mesh.name, rm);

                return rm;
            }
        }

        public bool drop_mesh(ref Mesh mesh)
        {
            if (register == null || !register.ContainsKey(mesh.name)) return false;
        
            RegisteredMesh rt = register[mesh.name];
            rt.Decrement();

            /*
        if (!rt.isactive() && false)
        {
            register.Remove(mesh.name);
            rt.destroy();
        }
        */
            return true;
        }

        public int Length()
        {
            return register.Count;
        }

        private void Clear()
        {
            if (register == null)
                return;

            while (register.Count > 0)
            {
                RemoveRegister(register.ElementAt(0).Key);
            }
        }

        public new void OnApplicationQuit()
        {
            Active = false;
            Clear();
            base.OnApplicationQuit();
        }

        private new void OnDestroy()
        {
            Active = false;
            Clear();
            base.OnDestroy();
        }

        private void RemoveRegister(string meshName)
        {
            if (!register.ContainsKey(meshName)) return;
        
            RegisteredMesh rt = register[meshName];
            register.Remove(meshName);
            rt.Destroy();
        }

#if UNITY_EDITOR
        public MeshPack PackMeshes()
        {
            var rMs = new MeshData[register.Count];
            var idx = 0;
            foreach(RegisteredMesh rm in register.Values)
            {
                rMs[idx] = rm.GetMeshData();
                idx++;
            }

            var packedMeshes = new MeshPack();
            packedMeshes.SetData(rMs);

            return packedMeshes;
        }
#endif
    }
}
