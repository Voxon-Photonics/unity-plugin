using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TextureRegisterTests
{
    [Test]
    public void accessTextureRegister()
    {
        TextureRegister TR = TextureRegister.Instance;
        Assert.IsNotNull(TR, "Texture Register not available");
        TR.ClearRegister();
    }

    [Test]
    public void getTile()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        Assert.IsNotNull(TT, "Tile Type not generated on initial request");
        TR.ClearRegister();
    }

    [Test]
    public void dropTileUnloading()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);
        bool dropSuccessful = TR.drop_tile(ref mat);

        Assert.True(dropSuccessful, "Tile was not unloaded from library");
        TR.ClearRegister();
    }

    [Test]
    public void dropTileDecrement()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        // Initial load
        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        // Additional load to increase "active" materials
        TT = TR.get_tile(ref mat);

        bool dropSuccessful = TR.drop_tile(ref mat);

        Assert.False(dropSuccessful, "Tile was incorrectly unloaded from library");
        TR.ClearRegister();
    }

    [Test]
    public void dropTileMissing()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        bool dropSuccessful = TR.drop_tile(ref mat);

        Assert.False(dropSuccessful, "Unloaded Tile was unloaded from library. Should be impossible.");
        TR.ClearRegister();
    }

    [Test]
    public void getTexturePointer()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        Assert.AreNotEqual(0, TT.first_pixel);
        TR.ClearRegister();
    }

    [Test]
    public void getHeight()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        Assert.AreEqual(256, TT.height);
        TR.ClearRegister();
    }

    [Test]
    public void getWidth()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        Assert.AreEqual(256, TT.width);
        TR.ClearRegister();
    }

    [Test]
    public void getPitch()
    {
        TextureRegister TR = TextureRegister.Instance;
        Material mat = TestObjects.tMaterial();

        Voxon.DLL.tiletype TT = TR.get_tile(ref mat);

        Assert.AreEqual(1024, TT.pitch);
        TR.ClearRegister();
    }
}
