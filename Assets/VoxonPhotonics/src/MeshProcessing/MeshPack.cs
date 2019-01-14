using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshPack : ISerializable
{

    [SerializeField]
    Voxon.MeshData[] data;

    public MeshPack()
    {
    }
	
    public void setData(Voxon.MeshData[] inData)
    {
        data = inData;
    }

    public Voxon.MeshData[] getData()
    {
        return data;
    }

    // Implement this method to serialize data. The method is called 
    // on serialization.
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // Use the AddValue method to specify serialized values.
        info.AddValue("props", data, typeof(RegisteredMesh[]));

    }

    // The special constructor is used to deserialize values.
    public MeshPack (SerializationInfo info, StreamingContext context)
    {
        // Reset the property value using the GetValue method.
        data = (Voxon.MeshData[])info.GetValue("props", typeof(Voxon.MeshData[]));
    }
}
