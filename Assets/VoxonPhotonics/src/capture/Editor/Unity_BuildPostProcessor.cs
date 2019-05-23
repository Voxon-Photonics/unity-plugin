using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Build.Reporting;

class Unity_BuildPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildReport report)
    {
		string file_name = "";
		string output_directory = "";

		foreach(BuildFile file in report.files)
		{
			
			if(file.role == "Executable")
			{
				file_name = System.IO.Path.GetFileName(file.path);
				output_directory = System.IO.Path.GetDirectoryName(file.path);
				Debug.Log("File: " + file_name + ", Folder: " + output_directory);
			}
		}

        // Generate Batch executable
        string batch_contents = string.Format("start \"\" \"{0}\" -batchmode", file_name);
        StreamWriter writer = new StreamWriter(output_directory + "\\VX.bat");

        try
        {
            writer.WriteLine(batch_contents);
        }
        catch (Exception E)
        {
            Debug.LogError("Unable to write batch file");
            Debug.LogError(E.Message);
        }
        finally
        {
            writer.Close();
        }
    }
}