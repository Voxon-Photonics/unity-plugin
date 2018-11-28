using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;


public class tTextureTests {
    [Test]
    public void createTTexture()
    {
        Texture2D testTexture = TestObjects.tTexture();
        Assert.IsNotNull(testTexture, "Texture not generated");
    }

    [Test]
    public void correctWidth()
    {
        Texture2D testTexture = TestObjects.tTexture();
        Assert.AreEqual(256, testTexture.width);
    }

    [Test]
    public void correctHeight()
    {
        Texture2D testTexture = TestObjects.tTexture();
        Assert.AreEqual(256, testTexture.height);
    }
}
