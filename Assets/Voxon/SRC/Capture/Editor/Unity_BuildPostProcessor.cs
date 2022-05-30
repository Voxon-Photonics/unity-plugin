using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Voxon
{
	/// <summary>
	/// Post build code to generate Batch file to run built program in batch mode
	/// </summary>
	class UnityBuildPostProcessor : IPostprocessBuildWithReport
	{
		public int callbackOrder => 0;

		public void OnPostprocessBuild(BuildReport report)
		{
			string fileName = "";
			string outputDirectory = "";

			Debug.Log("PostProcess Build");
			Debug.Log(report.files);

			foreach(BuildFile file in report.files)
			{
				if ((file.role == "Executable" || file.role == "executable" || file.role == "exe") && !file.path.Contains("UnityCrashHandler"))
				{
					fileName = Path.GetFileName(file.path);
					outputDirectory = Path.GetDirectoryName(file.path);
					Debug.Log($"File: {fileName}, Folder: {outputDirectory}");
					break;
				}
			}

			// Generate Batch executable
			string batchContents = $"start \"\" \"{fileName}\" -batchmode";
			StreamWriter writer = new StreamWriter(outputDirectory + "\\VX.bat");

			try
			{
				writer.WriteLine(batchContents);
			}
			catch (Exception e)
			{
				Debug.LogError("Unable to write batch file");
				Debug.LogError(e.Message);
			}
			finally
			{
				writer.Close();
			}
		}
	}
}