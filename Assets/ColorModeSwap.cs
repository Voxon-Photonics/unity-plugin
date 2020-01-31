using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class ColorModeSwap : MonoBehaviour
{
    // Start is called before the first frame update
    public ColorMode currentColor = ColorMode.RGB;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Voxon.Input.GetKeyDown("ColorSwap"))
        {
            currentColor--;
            if (currentColor < ColorMode.CYAN)
            {
                currentColor = ColorMode.RGB;
            }
            Debug.Log((currentColor.ToString()));
            VXProcess.Runtime.SetDisplayColor(currentColor);
        }
    }
}
