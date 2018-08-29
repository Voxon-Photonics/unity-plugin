using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class FillTester : MonoBehaviour
{
    public Vector3 grow = new Vector3(0.001f, 0.0004f, 0.001f);
    public string path = "./log.csv";
    float StdDevPrec = 0.05f;
    //Declare these in your class
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.5f;

    public float min_height = -4.1f;
    public float max_height = 4.1f;

    public int curret_step = 3;
    public int MAX_STEP = 3;

    

    // Voxon.VXTextComponent VXText;
    Voxon.VXComponent VXComponent;

    List<float> frames;

    float average;
    float standard_deviation;

    bool stablised = false;

    private void Start()
    {
        Reset();

        // VXText = FindObjectOfType<Voxon.VXTextComponent>();
        VXComponent = GetComponent<Voxon.VXComponent>();
        
    }

    public void Reset()
    {
        frames = new List<float>();
        stablised = false;
    }

    private void getStandardDeviation()
    {
        average = frames.Average();

        if (frames.Count < 2)
        {
            standard_deviation = 0;
            return;

        }

        if(Mathf.Abs(average - 10) < 0.01)
        {
            foreach (var frame in frames)
            {
                Debug.Log(frame);
            }
        }

        float sumOfDerivation = 0;
        foreach (float value in frames)
        {
            sumOfDerivation += (value) * (value);
        }
        float sumOfDerivationAverage = sumOfDerivation / (frames.Count - 1);
        float new_standard_deviation = Mathf.Sqrt(sumOfDerivationAverage - (average * average));

        if (!stablised && Mathf.Abs(standard_deviation - new_standard_deviation) < StdDevPrec)
        {
            Debug.Log("Stablised");
            stablised = true;
        }

        standard_deviation = new_standard_deviation;
    }

    void WriteLog()
    {
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
            // Create a file to write to.
            string createText = "VolumePercent,FrameRate,StdDev" + Environment.NewLine;
            File.WriteAllText(path, createText);
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        string appendText = (transform.localScale.x / 10).ToString() + "," + average + "," + standard_deviation + Environment.NewLine;
        File.AppendAllText(path, appendText);
    }

    void Update()
    {
        VXComponent.set_flag(curret_step);

        if (transform.localScale.x > 10.0)
        {
            Debug.Log("Finished");
            return;
        }

        if (VXComponent == null)
        {
            VXComponent = GetComponent<Voxon.VXComponent>();
            VXComponent.set_flag(curret_step);
        }

        m_timeCounter += Time.deltaTime;
        m_frameCounter++;

        if (m_timeCounter >= m_refreshTime)
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;

            frames.Add(m_lastFramerate);
            getStandardDeviation();
            if (stablised)
            {
                Debug.Log(average + ", +/- " + standard_deviation);
                //VXText.SetString(average + ", +/- " + standard_deviation);

                WriteLog();

                transform.localScale += grow;

                Reset();
            }
        }
    }
}
