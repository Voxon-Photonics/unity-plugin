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

        public tiletype get_tile(ref Texture2D in_texture)
        {
            if(_register == null)
            {
                _register = new Dictionary<string, RegisteredTile>();
            }


            RegisteredTile rt;
            if (_register.ContainsKey(in_texture.name))
            {
                rt = _register[in_texture.name];
                rt.Counter++;
                _register[in_texture.name] = rt;

                return rt.Texture;
            }
            else
            {
                rt = new RegisteredTile {Counter = 1, Texture = LoadTexture(ref in_texture)};
                _register.Add(in_texture.name, rt);
                return rt.Texture;
            }
        }

        public bool drop_tile(ref Texture2D texture)
        {
            if (_register == null || !_register.ContainsKey(texture.name)) return false;
        
            RegisteredTile rt = _register[texture.name];
            rt.Counter--;

            if(rt.Counter <= 0)
            {
                _register.Remove(texture.name);
                Marshal.FreeHGlobal(rt.Texture.first_pixel);
                return true;
            }

            _register[texture.name] = rt;
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

        tiletype LoadTexture(ref Texture2D in_texture)
        {
            //TextureFormat.BGRA32
            var reorderedTextures = new Texture2D(in_texture.width, in_texture.height, TextureFormat.BGRA32, false);

            Color32[] tCol = in_texture.GetPixels32();
            reorderedTextures.SetPixels32(tCol);

            var out_texture = new tiletype
            {
                height = (IntPtr) reorderedTextures.height,
                width = (IntPtr) reorderedTextures.width,
                pitch = (IntPtr) (reorderedTextures.width * Marshal.SizeOf(typeof(Color32))),
                first_pixel =
                    Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * reorderedTextures.GetRawTextureData().Length)
            };

            Marshal.Copy(reorderedTextures.GetRawTextureData(), 0, out_texture.first_pixel, reorderedTextures.GetRawTextureData().Length);

            Destroy(reorderedTextures);
            return out_texture;
        }
        
        public tiletype refresh_tile(ref Texture2D tex)
        {
            if(_register == null)
            {
                Debug.Log("New Dictionary");
                _register = new Dictionary<string, RegisteredTile>();
            }

            if (!_register.ContainsKey(tex.name))
            {
                return get_tile(ref tex);
            } else
            {
                return RefreshTexture(ref tex);
            }
        }
        
        tiletype RefreshTexture(ref Texture2D texture)
        {
            // Debug.Log("Refreshing Texture: " + texture.name);
            var reorderedTextures = new Texture2D(texture.width, texture.height, TextureFormat.BGRA32, false);

            Color32[] tCol = texture.GetPixels32();
            reorderedTextures.SetPixels32(tCol);

            RegisteredTile rt = _register[texture.name];
            Marshal.Copy(reorderedTextures.GetRawTextureData(), 0, rt.Texture.first_pixel, reorderedTextures.GetRawTextureData().Length);
            _register[texture.name] = rt;

            Destroy(reorderedTextures);
            return rt.Texture;
        }

    }
}
