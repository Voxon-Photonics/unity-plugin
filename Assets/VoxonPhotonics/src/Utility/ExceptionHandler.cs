using System;
using UnityEngine;

namespace Voxon
{
    public static class ExceptionHandler
    {
        public static void Except(String custom_message, Exception E)
        {
            if (custom_message != "")
            {
                Debug.LogError(custom_message + "\n" + E.Message);
            }
            else
            {
                Debug.LogError(E.Message);
            }

            Voxon.DLL.Shutdown();
        }
    }
}