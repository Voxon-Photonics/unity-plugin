using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using UnityEngine;

namespace Voxon
{
	public class Runtime : IRuntimePromise
	{
		string PluginFilePath = "";
		const string PluginFileName = "C#-Runtime.dll";
		const string PluginTypeName = "Voxon.IRuntimePromise";

		static Type tClassType;
		static object runtime;

		static Dictionary<String, MethodInfo> features;

		public Runtime()
		{
			features = new Dictionary<String, MethodInfo>();
			FindDLL();

			Assembly asm = Assembly.LoadFrom(PluginFilePath);

			tClassType = asm.GetType("Voxon.Runtime");
			runtime = Activator.CreateInstance(tClassType);

			var makeRequestMethod = tClassType.GetMethod("GetFeatures");
			HashSet<string> feature_names = (HashSet<string>)makeRequestMethod.Invoke(runtime, null);

			foreach(var feature in feature_names)
			{
				// Debug.Log("Loading:\t" + feature);
				features.Add(feature, tClassType.GetMethod(feature));
			}

		}


		public void RegistryPath(ref string PluginFilePath)
		{
			
		}

		private void FindDLL()
		{
			if (PluginFilePath == "")
			{
				if (PluginFilePath == "")
				{
					if (File.Exists(PluginFileName))
					{
						PluginFilePath = PluginFileName;
					}
				}

#if NET_4_6
				if (PluginFilePath == "")
				{
					var _dll = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon");
					if (_dll != null)
					{
						PluginFilePath = (string)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon").GetValue("Path") + PluginFileName;
					}
				}
#endif

				if (PluginFilePath == "")
				{
					string[] paths = Environment.GetEnvironmentVariable("Path").Split(';');

					foreach (var path in paths)
					{
						if (File.Exists(path + "\\" + PluginFileName))
						{
							PluginFilePath = path + "\\" + PluginFileName;
						}
					}
				}
			}
		}

		public void DrawBox(ref point3d min, ref point3d max, int fill, int colour)
		{
			features["DrawBox"].Invoke(runtime, new object[] { min, max, fill, colour });
		}

		public void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, int col)
		{
			features["DrawCube"].Invoke(runtime, new object[] { pp, pr, pd, pf, flags, col });
		}

		public void DrawGuidelines()
		{
			features["DrawGuidelines"].Invoke(runtime, null);
		}

		public void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int colorkey, int min_height, int flags)
		{
			object[] paras = new object[] { texture, pp, pr, pd, pf, colorkey, min_height, flags};
			features["DrawHeightmap"].Invoke(runtime, paras);
		}

		public void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, int col, byte[] text)
		{
			object[] paras = new object[] { pp, pr, pd, col, text};
			features["DrawLetters"].Invoke(runtime, paras);
		}

		public void DrawLine(ref point3d min, ref point3d max, int col)
		{
			object[] paras = new object[] { min, max, col };
			features["DrawLine"].Invoke(runtime, paras);
		}

		public void DrawPolygon(pol_t[] pt, int pt_count, int col)
		{
			object[] paras = new object[] { pt, pt_count, col };
			features["DrawPolygon"].Invoke(runtime, paras);
		}

		public void DrawSphere(ref point3d position, float radius, int issol, int colour)
		{
			object[] paras = new object[] { position, radius, issol, colour };
			features["DrawSphere"].Invoke(runtime, paras);
		}

		public void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags)
		{
			object[] paras = new object[] { texture, vertices, vertice_count, indices, indice_count, flags };
			features["DrawTexturedMesh"].Invoke(runtime, paras);
		}

		public void DrawUntexturedMesh(poltex[] vertices, int vertice_count, int[] indices, int indice_count, int flags, int colour)
		{
			object[] paras = new object[] { vertices, vertice_count, indices, indice_count, flags, colour };
			features["DrawUntexturedMesh"].Invoke(runtime, paras);
		}

		public void DrawVoxel(ref point3d position, int col)
		{
			object[] paras = new object[] { position, col };
			features["DrawVoxel"].Invoke(runtime, paras);
		}

		public void FrameEnd()
		{
			features["FrameEnd"].Invoke(runtime, null);
		}

		public bool FrameStart()
		{
			return (bool)features["FrameStart"].Invoke(runtime, null);
		}

		public float[] GetAspectRatio()
		{
			return (float[]) features["GetAspectRatio"].Invoke(runtime, null);

		}

		public float GetAxis(int axis, int player)
		{
			object[] paras = new object[] { axis, player };
			return (float)features["GetAxis"].Invoke(runtime, paras);
		}

		public bool GetButton(int button, int player)
		{
			object[] paras = new object[] { button, player };
			return (bool)features["GetButton"].Invoke(runtime, paras);

		}

		public bool GetButtonDown(int button, int player)
		{
			object[] paras = new object[] { button, player };
			return (bool)features["GetButtonDown"].Invoke(runtime, paras);
		}

		public bool GetButtonUp(int button, int player)
		{
			object[] paras = new object[] { button, player };
			return (bool)features["GetButtonUp"].Invoke(runtime, paras);
		}

		public HashSet<string> GetFeatures()
		{
			return (HashSet<string>)features["DrawPolygon"].Invoke(runtime, null);
		}

		public bool GetKey(int keycode)
		{
			object[] paras = new object[] { keycode };
			return (bool)features["GetKey"].Invoke(runtime, paras);
		}

		public bool GetKeyDown(int keycode)
		{
			object[] paras = new object[] { keycode };
			return (bool)features["GetKeyDown"].Invoke(runtime, paras);
		}

		public int GetKeyState(int keycode)
		{
			object[] paras = new object[] { keycode };
			return (int)features["GetKeyState"].Invoke(runtime, paras);
		}

		public bool GetKeyUp(int keycode)
		{
			object[] paras = new object[] { keycode };
			return (bool)features["GetKeyUp"].Invoke(runtime, paras);
		}

		public bool GetMouseButton(int button)
		{
			object[] paras = new object[] { button };
			return (bool)features["GetMouseButton"].Invoke(runtime, paras);
		}

		public bool GetMouseButtonDown(int button)
		{
			object[] paras = new object[] { button };
			return (bool)features["GetMouseButtonDown"].Invoke(runtime, paras);
		}

		public float[] GetMousePosition()
		{
			return (float[])features["GetMousePosition"].Invoke(runtime, null);
		}

		public float GetVolume()
		{
			return (float)features["GetVolume"].Invoke(runtime, null);
		}

		public void Initialise()
		{
			features["Initialise"].Invoke(runtime, null);
		}

		public bool isInitialised()
		{
			return (bool)features["isInitialised"].Invoke(runtime, null);
		}

		public bool isLoaded()
		{
			return (bool)features["isLoaded"].Invoke(runtime, null);
		}

		public void Load()
		{
			features["Load"].Invoke(runtime, null);
		}

		public void LogToFile(string msg)
		{
			object[] paras = new object[] { msg };
			features["LogToFile"].Invoke(runtime, paras);
		}

		public void LogToScreen(int x, int y, string Text)
		{
			object[] paras = new object[] { x, y, Text };
			features["LogToScreen"].Invoke(runtime, paras);
		}

		public void SetAspectRatio(float aspx, float aspy, float aspz)
		{
			object[] paras = new object[] { aspx, aspy, aspz };
			features["SetAspectRatio"].Invoke(runtime, paras);
		}

		public void Shutdown()
		{
			features["Shutdown"].Invoke(runtime, null);
		}

		public void Unload()
		{
			features["Unload"].Invoke(runtime, null);
		}
	}
}
