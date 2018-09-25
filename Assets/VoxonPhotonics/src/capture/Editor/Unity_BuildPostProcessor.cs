﻿using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using System.IO;
using System;

class Unity_BuildPostProcessor : IPostprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        string file_name = System.IO.Path.GetFileName(path);
        string output_directory = System.IO.Path.GetDirectoryName(path);

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