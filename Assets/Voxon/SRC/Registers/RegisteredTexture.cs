using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using System.Runtime.InteropServices;

namespace Voxon
{
	[Serializable]
	public class RegisteredTexture
	{
		public string name;
		public tiletype Texture;
		public int Counter = 1;

		public RegisteredTexture(ref Texture2D in_texture)
		{
			name = in_texture.name;
			//TextureFormat.BGRA32
			var reorderedTextures = new Texture2D(in_texture.width, in_texture.height, TextureFormat.BGRA32, false);

			Color32[] tCol = in_texture.GetPixels32();
			reorderedTextures.SetPixels32(tCol);

			var out_texture = new tiletype
			{
				height = (IntPtr)reorderedTextures.height,
				width = (IntPtr)reorderedTextures.width,
				pitch = (IntPtr)(reorderedTextures.width * Marshal.SizeOf(typeof(Color32))),
				first_pixel =
					Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * reorderedTextures.GetRawTextureData().Length)
			};

			Marshal.Copy(reorderedTextures.GetRawTextureData(), 0, out_texture.first_pixel, reorderedTextures.GetRawTextureData().Length);

			Texture = out_texture;
		}

		public RegisteredTexture(ref TextureData td)
		{
			name = td.name;
			Texture = td.Texture;
		}

#if UNITY_EDITOR
		public TextureData GetTextureData()
		{
			var textureData = new TextureData
			{
				name = name,
				Texture = Texture
			};

			return textureData;
		}
#endif
	}

}

