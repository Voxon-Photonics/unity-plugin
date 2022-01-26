using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSettings : MonoBehaviour
{

    float timer = 0;
    [Header("Density")]
    public bool densityExample = false;
    public float DensityCycleTime = 3; // Seconds
    float densityTarget = 4;
    float initialDensity = -1;


    [Header("Gamma")]
    public bool gammaExample = false;
    public float GammaCycleTime = 3; // Seconds
    float gammaTarget = 4;
    float initialGamma = -1;


    [Header("DotSize")]
    public bool dotSizeExample = false;
    public float DotSizeCycleTime = 3; // Seconds
    int dotSizeTarget = 4;
    int initialDotSize = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (densityExample)
        {
            if(initialDensity < 0)
			{
                initialDensity = Voxon.VXProcess.Runtime.GetDensity();
			}

            float newDensity = Mathf.Lerp(initialDensity, densityTarget, timer / DensityCycleTime);
            newDensity = Mathf.Clamp(newDensity, 0.1f, 4);
            Voxon.VXProcess.Runtime.SetDensity(newDensity);

            if (newDensity >= 4 || newDensity <= 0.1f)
            {
                if (densityTarget == 4)
                {
                    densityTarget = 0.1f;
                }
                else
                {
                    densityTarget = 4;
                }
                timer = 0;
                initialDensity = newDensity;
            }
        }

        if (gammaExample)
        {
            if (initialGamma < 0)
            {
                initialGamma = Voxon.VXProcess.Runtime.GetGamma();
            }

            float newGamma = Mathf.Lerp(initialGamma, gammaTarget, timer / GammaCycleTime);
            newGamma= Mathf.Clamp(newGamma, 0.1f, 4);

            Voxon.VXProcess.Runtime.SetGamma(newGamma);

            if (newGamma >= 4 || newGamma <= 0.1f)
            {
                if (gammaTarget == 4)
                {
                    gammaTarget = 0.1f;
                }
                else
                {
                    gammaTarget = 4;
                }
                timer = 0;
                initialGamma = newGamma;
            }
        }

        if (dotSizeExample)
        {
            if (initialDotSize < 0)
            {
                initialDotSize = Voxon.VXProcess.Runtime.GetDotSize();
            }

            float possible = Mathf.Lerp(initialDotSize, dotSizeTarget, timer / GammaCycleTime);
            int newDotSize = Mathf.RoundToInt(possible);

            if (newDotSize != Voxon.VXProcess.Runtime.GetDotSize())
			{
                Voxon.VXProcess.Runtime.SetDotSize(newDotSize);
            }


            if (newDotSize == dotSizeTarget)
            {
                if (dotSizeTarget == 4)
                {
                    dotSizeTarget = 0;
                }
                else
                {
                    dotSizeTarget = 4;
                }
                timer = 0;
                initialDotSize = newDotSize;
            }
        }

        /* Report Engine Values */
        float[] aspf = Voxon.VXProcess.Runtime.GetAspectRatio();
        Vector3 asp = new Vector3(aspf[0], aspf[1], aspf[2]);
        Voxon.VXProcess.add_log_line($"");
        Voxon.VXProcess.add_log_line($"");
        Voxon.VXProcess.add_log_line($"Gamma: {Voxon.VXProcess.Runtime.GetGamma()}");
        Voxon.VXProcess.add_log_line($"Density: {Voxon.VXProcess.Runtime.GetDensity()}");
        Voxon.VXProcess.add_log_line($"Dot Size: {Voxon.VXProcess.Runtime.GetDotSize()}");
        Voxon.VXProcess.add_log_line($"Aspect Ratio: {asp}");
        Voxon.VXProcess.add_log_line($"");
        Voxon.VXProcess.add_log_line($"");
        Voxon.VXProcess.add_log_line($"");
    }
}
