using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Voxon
{
    public class TextureRegister : Singleton<TextureRegister> {

        public struct RegisteredTile
        {
            public tiletype Texture;
            public int Counter;
        }

        private Dictionary<String, RegisteredTile> _register;

        // Use this for initialization
        private void Start () {
            _register = new Dictionary<string, RegisteredTile>();
        }

        public tiletype get_tile(ref Material mat)
        {
            if(_register == null)
            {
                _register = new Dictionary<string, RegisteredTile>();
            }


            RegisteredTile rt;
            if (_register.ContainsKey(mat.mainTexture.name))
            {
                rt = _register[(mat.mainTexture.name)];
                rt.Counter++;
                _register[(mat.mainTexture.name)] = rt;

                return rt.Texture;
            }
            else
            {
                rt = new RegisteredTile {Counter = 1, Texture = LoadTexture(ref mat)};
                _register.Add(mat.mainTexture.name, rt);
                return rt.Texture;
            }
        }

        public bool drop_tile(ref Material mat)
        {
            if (_register == null || !_register.ContainsKey(mat.mainTexture.name)) return false;
        
            RegisteredTile rt = _register[(mat.mainTexture.name)];
            rt.Counter--;

            if(rt.Counter <= 0)
            {
                _register.Remove(mat.mainTexture.name);
                Marshal.FreeHGlobal(rt.Texture.first_pixel);
                return true;
            }

            _register[(mat.mainTexture.name)] = rt;
            return false;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();

            ClearRegister();
        }

        private void RemoveRegister(string textureName)
        {
            if (!_register.ContainsKey(textureName)) return;
        
            RegisteredTile rt = _register[textureName];
            _register.Remove(textureName);
            Marshal.FreeHGlobal(rt.Texture.first_pixel);
        }

        public void ClearRegister()
        {
            if (_register == null)
                return;

            while (_register.Count > 0)
            {
                RemoveRegister(_register.ElementAt(0).Key);
            }
        }

        tiletype LoadTexture(ref Material mat)
        {
            //TextureFormat.BGRA32
            var reorderedTextures = new Texture2D(((Texture2D) mat.mainTexture).width, ((Texture2D) mat.mainTexture).height, TextureFormat.BGRA32, false);

            Color32[] tCol = ((Texture2D) mat.mainTexture)?.GetPixels32();
            reorderedTextures.SetPixels32(tCol);

            var texture = new tiletype
            {
                height = (IntPtr) reorderedTextures.height,
                width = (IntPtr) reorderedTextures.width,
                pitch = (IntPtr) (reorderedTextures.width * Marshal.SizeOf(typeof(Color32))),
                first_pixel =
                    Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * reorderedTextures.GetRawTextureData().Length)
            };

            Marshal.Copy(reorderedTextures.GetRawTextureData(), 0, texture.first_pixel, reorderedTextures.GetRawTextureData().Length);

            return texture;
        }

    }
}
