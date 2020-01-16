using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UnityEngine;

namespace Voxon
{
	public class Runtime : IRuntimePromise
	{
		private string _pluginFilePath = "";
		private const string PluginFileName = "C#-Runtime.dll";
		
		private const string PluginTypeName = "Voxon.Runtime";
		private const string HelixTypeName = "Voxon.HelixRuntime";

		public string ActiveRuntime;
		
		private static Type _tClassType;
		private static object _runtime;

		private static Dictionary<string, MethodInfo> _features;

		public Runtime()
		{
			_features = new Dictionary<string, MethodInfo>();
			FindDll();

			Assembly asm = Assembly.LoadFrom(_pluginFilePath);

			_tClassType = asm.GetType(HelixTypeName);
			ActiveRuntime = HelixTypeName;
			if (_tClassType == null)
			{
				Debug.LogWarning("Helix Interface Not Available. SDK Out of date");
				_tClassType = asm.GetType(PluginTypeName);
				ActiveRuntime = PluginTypeName;
			}

			_runtime = Activator.CreateInstance(_tClassType);

			MethodInfo makeRequestMethod = _tClassType.GetMethod("GetFeatures");
			if (makeRequestMethod == null) return;

			var featureNames = (HashSet<string>) makeRequestMethod.Invoke(_runtime, null);

			foreach (string feature in featureNames)
			{
				_features.Add(feature, _tClassType.GetMethod(feature));
			}
			
		}

		private void FindDll()
		{
			if (_pluginFilePath != "") return;
			
			if (File.Exists(PluginFileName))
			{
				_pluginFilePath = PluginFileName;
				return;
			}

#if NET_4_6
			RegistryKey dll = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon");
			if (dll != null)
			{
				_pluginFilePath =
					$"{(string) Registry.LocalMachine.OpenSubKey("SOFTWARE\\Voxon\\Voxon")?.GetValue("Path")}{PluginFileName}";
				return;
			}
			
#endif
			
			string[] paths = Environment.GetEnvironmentVariable("Path")?.Split(';');

			if (paths == null) return;
			
			foreach (string path in paths)
			{
				if (!File.Exists($"{path}\\{PluginFileName}")) continue;
				
				_pluginFilePath = $"{path}\\{PluginFileName}";
			}
		}

		public void DrawBox(ref point3d min, ref point3d max, int fill, int colour)
		{
			_features["DrawBox"].Invoke(_runtime, parameters: new object[] { min, max, fill, colour });
		}

		public void DrawCube(ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int flags, int col)
		{
			_features["DrawCube"].Invoke(_runtime, new object[] { pp, pr, pd, pf, flags, col });
		}

		public void DrawGuidelines()
		{
			_features["DrawGuidelines"].Invoke(_runtime, null);
		}

		public void DrawHeightmap(ref tiletype texture, ref point3d pp, ref point3d pr, ref point3d pd, ref point3d pf, int colorkey, int minHeight, int flags)
		{
			var paras = new object[] { texture, pp, pr, pd, pf, colorkey, minHeight, flags};
			_features["DrawHeightmap"].Invoke(_runtime, paras);
		}

		public void DrawLetters(ref point3d pp, ref point3d pr, ref point3d pd, int col, byte[] text)
		{
			var paras = new object[] { pp, pr, pd, col, text};
			_features["DrawLetters"].Invoke(_runtime, paras);
		}

		public void DrawLine(ref point3d min, ref point3d max, int col)
		{
			var paras = new object[] { min, max, col };
			_features["DrawLine"].Invoke(_runtime, paras);
		}

		public void DrawPolygon(pol_t[] pt, int ptCount, int col)
		{
			var paras = new object[] { pt, ptCount, col };
			_features["DrawPolygon"].Invoke(_runtime, paras);
		}

		public void DrawSphere(ref point3d position, float radius, int issol, int colour)
		{
			var paras = new object[] { position, radius, issol, colour };
			_features["DrawSphere"].Invoke(_runtime, paras);
		}

		public void DrawTexturedMesh(ref tiletype texture, poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags)
		{
			var paras = new object[] { texture, vertices, verticeCount, indices, indiceCount, flags };
			_features["DrawTexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawUntexturedMesh(poltex[] vertices, int verticeCount, int[] indices, int indiceCount, int flags, int colour)
		{
			var paras = new object[] { vertices, verticeCount, indices, indiceCount, flags, colour };
			_features["DrawUntexturedMesh"].Invoke(_runtime, paras);
		}

		public void DrawVoxel(ref point3d position, int col)
		{
			var paras = new object[] { position, col };
			_features["DrawVoxel"].Invoke(_runtime, paras);
		}

		public void FrameEnd()
		{
			_features["FrameEnd"].Invoke(_runtime, null);
		}

		public bool FrameStart()
		{
			return (bool)_features["FrameStart"].Invoke(_runtime, null);
		}

		public float[] GetAspectRatio()
		{
			return (float[]) _features["GetAspectRatio"].Invoke(_runtime, null);

		}

		public float GetAxis(int axis, int player)
		{
			var paras = new object[] { axis, player };
			return (float)_features["GetAxis"].Invoke(_runtime, paras);
		}

		public bool GetButton(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButton"].Invoke(_runtime, paras);

		}

		public bool GetButtonDown(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonDown"].Invoke(_runtime, paras);
		}

		public bool GetButtonUp(int button, int player)
		{
			var paras = new object[] { button, player };
			return (bool)_features["GetButtonUp"].Invoke(_runtime, paras);
		}

		public HashSet<string> GetFeatures()
		{
			return (HashSet<string>)_features["DrawPolygon"].Invoke(_runtime, null);
		}

		public bool GetKey(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKey"].Invoke(_runtime, paras);
		}

		public bool GetKeyDown(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyDown"].Invoke(_runtime, paras);
		}

		public int GetKeyState(int keycode)
		{
			var paras = new object[] { keycode };
			return (int)_features["GetKeyState"].Invoke(_runtime, paras);
		}

		public bool GetKeyUp(int keycode)
		{
			var paras = new object[] { keycode };
			return (bool)_features["GetKeyUp"].Invoke(_runtime, paras);
		}

		public bool GetMouseButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButton"].Invoke(_runtime, paras);
		}

		public bool GetMouseButtonDown(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetMouseButtonDown"].Invoke(_runtime, paras);
		}

		public float[] GetMousePosition()
		{
			return (float[])_features["GetMousePosition"].Invoke(_runtime, null);
		}

		public float[] GetSpaceNavPosition()
		{
			return (float[])_features["GetSpaceNavPosition"].Invoke(_runtime, null);
		}
		
		public float[] GetSpaceNavRotation()
		{
			return (float[])_features["GetSpaceNavRotation"].Invoke(_runtime, null);
		}
		
		public bool GetSpaceNavButton(int button)
		{
			var paras = new object[] { button };
			return (bool)_features["GetSpaceNavButton"].Invoke(_runtime, paras);
		}

		public float GetVolume()
		{
			return (float)_features["GetVolume"].Invoke(_runtime, null);
		}

		public void Initialise()
		{
			_features["Initialise"].Invoke(_runtime, null);
		}

		public bool isInitialised()
		{
			return (bool)_features["isInitialised"].Invoke(_runtime, null);
		}

		public bool isLoaded()
		{
			return (bool)_features["isLoaded"].Invoke(_runtime, null);
		}

		public void Load()
		{
			_features["Load"].Invoke(_runtime, null);
		}

		public void LogToFile(string msg)
		{
			var paras = new object[] { msg };
			_features["LogToFile"].Invoke(_runtime, paras);
		}

		public void LogToScreen(int x, int y, string text)
		{
			var paras = new object[] { x, y, text };
			_features["LogToScreen"].Invoke(_runtime, paras);
		}

		public void SetAspectRatio(float aspx, float aspy, float aspz)
		{
			var paras = new object[] { aspx, aspy, aspz };
			_features["SetAspectRatio"].Invoke(_runtime, paras);
		}

		public void Shutdown()
		{
			_features["Shutdown"].Invoke(_runtime, null);
		}

		public void Unload()
		{
			_features["Unload"].Invoke(_runtime, null);
		}

		public long GetDLLVersion()
		{
			return (long)_features["GetDLLVersion"].Invoke(_runtime, null);
		}

		public string GetSDKVersion()
		{
			return (string) _features["GetSDKVersion"].Invoke(_runtime, null);
		}

		public void SetHelixMode(bool helixMode)
		{
			if (_features.ContainsKey("SetHelixMode"))
			{
				var paras = new object[] { helixMode };
				_features["SetHelixMode"].Invoke(_runtime, paras);
				
			}
		}
		public bool GetHelixMode()
		{
			return _features.ContainsKey("GetHelixMode") && (bool) _features["GetHelixMode"].Invoke(_runtime, null);
		}
		
		public float GetExternalRadius()
		{
			if (_features.ContainsKey("GetExternalRadius"))
			{
				return (float) _features["GetExternalRadius"].Invoke(_runtime, null);
			}

			return 0.0f;
		}
	}
}
