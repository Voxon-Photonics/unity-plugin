using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    [Serializable]
    public class MeshData
    {
        // Mesh Data
        [SerializeField]
        public poltex[] vertices;
        [SerializeField]
        public string name;
        [FormerlySerializedAs("vertex_count")] [SerializeField]
        public int vertexCount;        // Number of vertices
        [FormerlySerializedAs("submesh_count")] [SerializeField]
        public int submeshCount;      // Count of Submeshes part of mesh
        [SerializeField]
        public int[][] indices;
        [FormerlySerializedAs("index_counts")] [SerializeField]
        public int[] indexCounts;
    }
}

