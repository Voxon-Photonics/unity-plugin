using UnityEngine;
using Voxon;
using System;

// Draw Guides Script
/* 31/ 07 / 2023 - Matthew Vecchio For Voxon
 * A Voxon x Unity script for showing various guides on the volumetric display
 * 
 */ 

public class DrawGuides : MonoBehaviour, IDrawable // If you want to write directly to the VoxieBox inherit the IDrawable Class
    {
        point3d pp0, pp1;
        float[] point3dArray = new float[3];
        public bool DrawGuidesBox = true;
        public bool DrawGuidesHelix = false;
        public bool DrawGuidesCenter = false;

        int boxCol = 0xff0000;
        int helixCol = 0x00ffff;
        int centerColor = 0xffff00;

        int SetColor(int rValue, int gValue, int bValue)
        {
            return (rValue << 16) | (gValue << 8) | (bValue);
        }


        // Start is called before the first frame update
        void Start()
        {
            VXProcess.Drawables.Add(this); // if you want to write directly to the VoxieBox you need to add the script to the  'Drawables' array
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Draw()
        {
            if (DrawGuidesBox)   drawGuideBox(boxCol);
            if (DrawGuidesHelix) drawGuideHelix(helixCol);
            if (DrawGuidesCenter) drawGuidesCenter(centerColor);
        }

        void drawGuidesCenter(int color)
        {
            pp0.x = 0;
            pp0.y = 0;
            pp0.z = 0;

            VXProcess.Runtime.DrawSphere(ref pp0, 0.15f, 0, color);
            VXProcess.Runtime.DrawSphere(ref pp0, 0.01f, 1, color);

        }
        void drawGuideBox(int color)
        {

            point3dArray = VXProcess.Runtime.GetAspectRatio();
            pp0.x = -point3dArray[0] ;
            pp0.y = -point3dArray[1] ;
            pp0.z = point3dArray[2] ;

            pp1.x = point3dArray[0] ;
            pp1.y = point3dArray[1] ;
            pp1.z = -point3dArray[2] ;

            VXProcess.Runtime.DrawBox(ref pp0, ref pp1, 1, color);
        }

        void drawGuideHelix(int color)
        {

        // if spinner draw around the outside of the shape
        int n = 64;
        int j = 0;
        int i = 0;
        point3dArray = VXProcess.Runtime.GetAspectRatio();

        float rad = point3dArray[0] * .99f;
        float aspz = point3dArray[2];


        for (j = -64; j <= 64; j++)
            {
                if (j == -62) j = 62;
                for (i = 0; i < n; i++)
                {

                    pp0.x = (float)Math.Cos((i + 0) * Math.PI * 2.0 / n) * (rad);
                    pp0.y = (float)Math.Sin((i + 0) * Math.PI * 2.0 / n) * (rad);
                    pp0.z = (float)j * aspz / 64f;

                    pp1.x = (float)Math.Cos((float)(i + 1) * Math.PI * 2.0 / (float)n) * (rad);
                    pp1.y = (float)Math.Sin((float)(i + 1) * Math.PI * 2.0 / (float)n) * (rad);
                    pp1.z = (float)j * aspz / 64f;


                    VXProcess.Runtime.DrawLine(ref pp0, ref pp1, color);

                }
            }

        n = 32;
        for (i = 0; i < n; i++)
            {

            
                pp0.x = (float)Math.Cos((float)(i + 0) * Math.PI * 2.0 / (float)n) * (rad);
                pp0.y = (float)Math.Sin((float)(i + 0) * Math.PI * 2.0 / (float)n) * (rad);
                pp0.z = -aspz;

                pp1.x = (float)Math.Cos((float)(i + 0) * Math.PI * 2.0 / (float)n) * (rad);
                pp1.y = (float)Math.Sin((float)(i + 0) * Math.PI * 2.0 / (float)n) * (rad);
                pp1.z = +aspz;

                VXProcess.Runtime.DrawLine(ref pp0, ref pp1, color);
            
            }

        }
    }

