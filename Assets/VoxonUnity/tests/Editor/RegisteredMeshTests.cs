using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RegisteredMeshTests
{

    [Test]
    public void createRegisteredMesh()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);
        Assert.IsNotNull(testRM, "Registered Mesh was not created from Test Game Object");
    }

    [Test]
    public void hasName()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Assert.AreEqual("tGameObject.Name", testRM.name);
    }

    [Test]
    public void countsSelf()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Assert.AreEqual(1, testRM.counter);
    }

    [Test]
    public void incrementsCounter()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);
        int old_counter = testRM.counter;
        testRM.increment();

        // Test value has changed (incase we accidentally set the default value to 2)
        Assert.AreNotEqual(old_counter, testRM.counter);

        // Test value is now 2
        Assert.AreEqual(2, testRM.counter);
    }

    [Test]
    public void decrementsCounter()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);
        int old_counter = testRM.counter;
        testRM.decrement();

        // Test value has changed
        Assert.AreNotEqual(old_counter, testRM.counter);

        // Test value is now 0
        Assert.AreEqual(0, testRM.counter);
    }

    [Test]
    public void registeredMeshIsActive()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);
        Assert.IsTrue(testRM.isactive());
    }

    [Test]
    public void registeredMeshIsDeactivated()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        testRM.counter = 0;
        Assert.IsFalse(testRM.isactive());
    }

    [Test]
    public void buildPoltex()
    {
        Voxon.DLL.poltex_t pol = RegisteredMesh.build_poltex(new Vector3(1, 2, 3), new Vector2(4, 5), 6);

        Voxon.DLL.poltex_t expected_pol = new Voxon.DLL.poltex_t();
        expected_pol.x = 1;
        expected_pol.y = 2;
        expected_pol.z = 3;
        expected_pol.u = 4;
        expected_pol.v = 5;
        expected_pol.col = 6;

        // Poltex buid is direct mapping (no transforms into Voxon Space)
        Assert.AreEqual(expected_pol, pol);
    }

    [Test]
    public void loadVertices()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Vector3[] vertices = mesh.vertices;

        for (int idx = mesh.vertexCount - 1; idx >= 0; --idx)
        {
            Assert.AreEqual(vertices[idx].x, testRM.vertices[idx].x);
            Assert.AreEqual(vertices[idx].y, testRM.vertices[idx].y);
            Assert.AreEqual(vertices[idx].z, testRM.vertices[idx].z);
        }
    }

    [Test]
    public void loadUvs()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        System.Collections.Generic.List<Vector2> uvs = new System.Collections.Generic.List<Vector2>();
        mesh.GetUVs(0, uvs);

        for (int idx = mesh.uv.Length - 1; idx >= 0; --idx)
        {
            Assert.AreEqual(uvs[idx].x, testRM.vertices[idx].u);
            Assert.AreEqual(uvs[idx].y, testRM.vertices[idx].v);
        }
    }

    [Test]
    public void loadIndices()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        int[] indices = mesh.GetIndices(0);
        int _indices = indices.Length;

        for (int i = 0, out_idx = 0; i < _indices; i += 3, out_idx += 4)
        {
            // Copy internal array to output array
            Assert.AreEqual(indices[i + 0], testRM.indices[0][0 + out_idx]);
            Assert.AreEqual(indices[i + 1], testRM.indices[0][1 + out_idx]);
            Assert.AreEqual(indices[i + 2], testRM.indices[0][2 + out_idx]);

            // flag end of triangle
            Assert.AreEqual(-1, testRM.indices[0][3 + out_idx]);
        }
    }

    [Test]
    public void computeTransformCPU_Identity()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Voxon.DLL.poltex_t[] vt = new Voxon.DLL.poltex_t[mesh.vertexCount];

        Matrix4x4 mat = Matrix4x4.identity;

        testRM.compute_transform_cpu(mat, ref vt);

        /* Original Vertices before VX1 Swap */
        /*
        Vector3[] expected_vertices = {
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 0.5f, 1f),
            new Vector3(-1f, 0.5f, 1f)
        };
        */

        Vector3[] expected_vertices = {
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, -1f, 1f),
            new Vector3(-1f, -1f, 1f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(1f, -1f, -0.5f),
            new Vector3(-1f, -1f, -0.5f)
        };

        for (int idx = 0; idx < mesh.vertexCount; idx++)
        {
            Assert.AreEqual(expected_vertices[idx].x, vt[idx].x, "X failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].y, vt[idx].y, "Y failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].z, vt[idx].z, "Z failed on index: " + idx);
        }
    }

    [Test]
    public void computeTransformCPU_Scale()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Voxon.DLL.poltex_t[] vt = new Voxon.DLL.poltex_t[mesh.vertexCount];

        Matrix4x4 mat = Matrix4x4.Scale(new Vector3(2, 2, 2));

        testRM.compute_transform_cpu(mat, ref vt);

        Vector3[] expected_vertices = {
            new Vector3(2f, 2f, 2f),
            new Vector3(2f, -2f, 2f),
            new Vector3(-2f, -2f, 2f),
            new Vector3(-2f, 2f, 2f),
            new Vector3(2f, -2f, -1f),
            new Vector3(-2f, -2f, -1f)
        };

        for (int idx = 0; idx < mesh.vertexCount; idx++)
        {
            Assert.AreEqual(expected_vertices[idx].x, vt[idx].x, "X failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].y, vt[idx].y, "Y failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].z, vt[idx].z, "Z failed on index: " + idx);
        }
    }

    [Test]
    public void computeTransformCPU_Translate()
    {
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Voxon.DLL.poltex_t[] vt = new Voxon.DLL.poltex_t[mesh.vertexCount];

        Matrix4x4 mat = Matrix4x4.Translate(new Vector3(2, 2, 2));

        testRM.compute_transform_cpu(mat, ref vt);

        Vector3[] expected_vertices = {
            new Vector3(3f, -1f, -1f),
            new Vector3(3f, -3f, -1f),
            new Vector3(1f, -3f, -1f),
            new Vector3(1f, -1f, -1f),
            new Vector3(3f, -3f, -2.5f),
            new Vector3(1f, -3f, -2.5f)
        };

        for (int idx = 0; idx < mesh.vertexCount; idx++)
        {
            Assert.AreEqual(expected_vertices[idx].x, vt[idx].x, "X failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].y, vt[idx].y, "Y failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].z, vt[idx].z, "Z failed on index: " + idx);
        }
    }

    [Test]
    public void computeTransformCPU_Rotation()
    {
        // Perform a 90 degree rotation around the X axis
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Voxon.DLL.poltex_t[] vt = new Voxon.DLL.poltex_t[mesh.vertexCount];

        Matrix4x4 mat = Matrix4x4.Rotate(Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)));

        testRM.compute_transform_cpu(mat, ref vt);

        Vector3[] expected_vertices = {
            new Vector3(1,-1,1),
            new Vector3(1,-1,-1),
            new Vector3(-1,-1,-1),
            new Vector3(-1,-1,1),
            new Vector3(1,0.5f,-1),
            new Vector3(-1,0.5f,-1),
        };

        for (int idx = 0; idx < mesh.vertexCount; idx++)
        {
            Assert.AreEqual(expected_vertices[idx].x, vt[idx].x, 0.001f, "X failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].y, vt[idx].y, 0.001f, "Y failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].z, vt[idx].z, 0.001f, "Z failed on index: " + idx);
        }
    }

    [Test]
    public void computeTransformCPU_LocalMatrix()
    {
        // Perform a 90 degree rotation around the X axis
        GameObject go = TestObjects.tGameObject();
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        RegisteredMesh testRM = new RegisteredMesh(ref mesh);

        Voxon.DLL.poltex_t[] vt = new Voxon.DLL.poltex_t[mesh.vertexCount];

        Matrix4x4 mat = Matrix4x4.TRS(new Vector3(2, 2, 2), Quaternion.AngleAxis(90, new Vector3(1, 0, 0)), new Vector3(2, 2, 2));

        testRM.compute_transform_cpu(mat, ref vt);

        // Unity Rotation is done Right Handed
        Vector3[] expected_vertices = {
            new Vector3(4,0,-4),
            new Vector3(4,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,-4),
            new Vector3(4,-3,0),
            new Vector3(0,-3,0)
        };

        for (int idx = 0; idx < mesh.vertexCount; idx++)
        {
            Assert.AreEqual(expected_vertices[idx].x, vt[idx].x, 0.001f, "X failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].y, vt[idx].y, 0.001f, "Y failed on index: " + idx);
            Assert.AreEqual(expected_vertices[idx].z, vt[idx].z, 0.001f, "Z failed on index: " + idx);
        }
    }
}
