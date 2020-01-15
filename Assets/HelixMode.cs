using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;

public class HelixMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private float delay = 3;
    private float remaining = 0;
    void Update()
    {
        remaining -= Time.deltaTime;
        if (Voxon.Input.GetKey("Helix") && remaining < 0.1)
        {
            bool is_helix_mode = VXProcess.Runtime.GetHelixMode();

            VXProcess.Runtime.SetHelixMode(!is_helix_mode);
            remaining = delay;
            Debug.Log("Helix Mode set to " + !is_helix_mode);
            if (!is_helix_mode)
            {
                Debug.Log("Radius = " + VXProcess.Runtime.GetExternalRadius());
            }
        }
    }
}
