using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class tGameObjectTests {

    [Test]
    public void createTGameObject()
    {
        GameObject testObject = TestObjects.tGameObject();
        Assert.IsNotNull(testObject, "GameObject not generated");
    }

    [Test]
    public void tGameObjectHasMeshFilter()
    {
        GameObject testObject = TestObjects.tGameObject();
        MeshFilter meshFilter = testObject.GetComponent<MeshFilter>();

        Assert.IsNotNull(meshFilter, "MeshFilter not generated");
    }

    [Test]
    public void tGameObjectHasMesh()
    {
        GameObject testObject = TestObjects.tGameObject();
        testObject.GetComponent<MeshFilter>();
        Mesh mesh = testObject.GetComponent<MeshFilter>().sharedMesh;

        Assert.IsNotNull(mesh, "Mesh not generated");
    }

    [Test]
    public void tGameObjectHasSingleSubmesh()
    {
        GameObject testObject = TestObjects.tGameObject();
        testObject.GetComponent<MeshFilter>();
        Mesh mesh = testObject.GetComponent<MeshFilter>().sharedMesh;

        Assert.AreEqual(1, mesh.subMeshCount);
    }
}
