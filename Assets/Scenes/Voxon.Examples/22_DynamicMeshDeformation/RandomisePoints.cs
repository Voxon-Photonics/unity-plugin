using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RandomisePoints : MonoBehaviour
{
	MeshFilter mf;
	Mesh mesh;
	// Start is called before the first frame update
	void Start()
    {
		mf = GetComponent<MeshFilter>();
		mesh = mf.mesh;
	}

    // Update is called once per frame
    void Update()
    {
		Vector3[] verts = mesh.vertices;
		int idx = Random.Range(0, verts.Length);
		verts[idx] = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
		mesh.SetVertices(verts);
		mf.mesh = mesh;
	}
}
