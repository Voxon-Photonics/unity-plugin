using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TextureRegister : Singleton<TextureRegister> {

    public struct registered_tile
    {
        public Voxon.DLL.tiletype texture;
        public int counter;
    }

    private Dictionary<String, registered_tile> Register;

    // Use this for initialization
    void Start () {
        Register = new Dictionary<String, registered_tile>();
    }

    public Voxon.DLL.tiletype get_tile(ref Material mat)
    {
        if(Register == null)
        {
            Register = new Dictionary<String, registered_tile>();
        }


        if (Register.ContainsKey(mat.mainTexture.name))
        {
            registered_tile rt = Register[(mat.mainTexture.name)];
            rt.counter++;
            Register[(mat.mainTexture.name)] = rt;

            return rt.texture;
        }
        else
        {
            registered_tile rt = new registered_tile();
            rt.counter = 1;
            rt.texture = LoadTexture(ref mat);
            Register.Add(mat.mainTexture.name, rt);
            return rt.texture;
        }
    }

    public bool drop_tile(ref Material mat)
    {
        if (Register.ContainsKey(mat.mainTexture.name))
        {
            registered_tile rt = Register[(mat.mainTexture.name)];
            rt.counter--;

            if(rt.counter <= 0)
            {
                Register.Remove(mat.mainTexture.name);
                Marshal.FreeHGlobal(rt.texture.first_pixel);
                return true;
            }

            Register[(mat.mainTexture.name)] = rt;
        }
        return false;
    }

    private new void OnDestroy()
    {
        base.OnDestroy();

        ClearRegister();
    }

    private void RemoveRegister(string name)
    {
        if (!Register.ContainsKey(name))
        {
            return;
        }
        else
        {
            registered_tile rt = Register[name];
            Register.Remove(name);
            Marshal.FreeHGlobal(rt.texture.first_pixel);
        }
    }

    public void ClearRegister()
    {
        if (Register == null)
            return;

        while (Register.Count > 0)
        {
            RemoveRegister(Register.ElementAt(0).Key);
        }
    }

    Voxon.DLL.tiletype LoadTexture(ref Material mat)
    {
        //TextureFormat.BGRA32
        Texture2D reordered_textures = new Texture2D((mat.mainTexture as Texture2D).width, (mat.mainTexture as Texture2D).height, TextureFormat.BGRA32, false);

        Color32[] t_col = (mat.mainTexture as Texture2D).GetPixels32();
        reordered_textures.SetPixels32(t_col);

        Voxon.DLL.tiletype texture = new Voxon.DLL.tiletype();

        texture.height = (IntPtr)reordered_textures.height;
        texture.width = (IntPtr)reordered_textures.width;
        texture.pitch = (IntPtr)(reordered_textures.width * Marshal.SizeOf(typeof(Color32)));
        texture.first_pixel = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * reordered_textures.GetRawTextureData().Length);
        Marshal.Copy(reordered_textures.GetRawTextureData(), 0, texture.first_pixel, reordered_textures.GetRawTextureData().Length);

        return texture;
    }

}
