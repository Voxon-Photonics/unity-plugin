using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  A simple script to demonstrate how to toggle the Voxon volumetric display on and off
 * 27/05/2022 Matthew Vecchio
 */

public class DisplayToggle : MonoBehaviour
{
    bool isCurrentDisplay3D = true;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       

        // Voxon Input 'SpaceBar" has been registered to Voxon.InputManager  (Located Voxon -> Input Manager in top menu)
        if (Voxon.Input.GetKeyDown("SpaceBar"))
        {

            if (isCurrentDisplay3D)
            {
               // Voxon.VXProcess.Runtime.SetDisplay2D() is a function which allows you to set the display to off
               Voxon.VXProcess.Runtime.SetDisplay2D();
               isCurrentDisplay3D = false;
               Voxon.VXProcess.add_log_line("Screen is now 2D");

            } else
            {
                // Voxon.VXProcess.Runtime.SetDisplay3D() is a function which allows you to set the display to on
               Voxon.VXProcess.Runtime.SetDisplay3D();
               Voxon.VXProcess.add_log_line("Screen is now 3D");
               isCurrentDisplay3D = true;
            }
        }


        // 31/1/2023 New Feature : to hibernate the display pass in the LED values to reduce the image to (0 is off... something like 30 is low).
        // This ensures the projector doesn't overheat when left on 2D display mode for long periods of time. 
                


        // Voxon Input 'H" has been registered to Voxon.InputManager  (Located Voxon -> Input Manager in top menu)
        if (Voxon.Input.GetKeyDown("H")) 
        {

            if (isCurrentDisplay3D)
            {
                Voxon.VXProcess.Runtime.SetDisplay2D(30); 
                Voxon.VXProcess.add_log_line("Screen is now 2D and Hibernating");
                isCurrentDisplay3D = false;
            }
            else
            {
            
                Voxon.VXProcess.Runtime.SetDisplay3D();
                Voxon.VXProcess.add_log_line("Screen is now 3D and Hibernating");
                isCurrentDisplay3D = true;
            }
        }

    }
}
