using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon
{
    [Serializable]
    public class MeshData
    {
        // Mesh Data
        [SerializeField]
        public Voxon.DLL.poltex_t[] vertices;
        [SerializeField]
        public string name;
        [SerializeField]
        public int vertex_count;        // Number of vertices
        [SerializeField]
        public int submesh_count;      // Count of Submeshes part of mesh
        [SerializeField]
        public int[][] indices;
        [SerializeField]
        public int[] index_counts;
    }
}

