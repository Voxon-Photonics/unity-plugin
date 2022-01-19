using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
	MeshFilter mf;
	MeshRenderer mr;

    Vector3[] vertices;
    int[] triangles;

    void Start()
    {
		mf = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;

		mr = gameObject.AddComponent<MeshRenderer>();

        CreateShape();
        UpdateMesh();

		gameObject.AddComponent<Voxon.VXDynamicComponent>();
	}

	void CreateShape()
    {
        vertices = new Vector3[]
        {

            new Vector3 (0,0,0),
            new Vector3 (0,0,1),
            new Vector3 (1,0,0),
            new Vector3 (1,0,1)


        };

        triangles = new int[]
        {
            0,1,2,
            1,3,2
        };

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
