using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate_Reporter : MonoBehaviour {
    float StdDevPrec = 0.05f;
    //Declare these in your class
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    public float m_refreshTime = 0.5f;

    List<float> frames;

    float average;
    float standard_deviation;

    bool stabalised = false;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        frames = new List<float>();
        stabalised = false;
    }

    private void getStandardDeviation()
    {
        average = frames.Average();
        if(frames.Count < 2)
        {
            standard_deviation = 0;
            return;

        }

        float sumOfDerivation = 0;
        foreach (float value in frames)
        {
            sumOfDerivation += (value) * (value);
        }
        float sumOfDerivationAverage = sumOfDerivation / (frames.Count - 1);
        float new_standard_deviation = Mathf.Sqrt(sumOfDerivationAverage - (average * average));
        if(!stabalised && Mathf.Abs(standard_deviation - new_standard_deviation) < StdDevPrec)
        {
            Debug.Log("Stablised");
            stabalised = true;
        }

        standard_deviation = new_standard_deviation;
    }

    void Update()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
            m_lastFramerate = m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;

            frames.Add(m_lastFramerate);
            getStandardDeviation();
            if(stabalised)
            {
                // Debug.Log(average + ", +/- " + standard_deviation);
                VXProcess.Instance.add_log_line(average + ", +/- " + standard_deviation);
            }
            else
            {
                VXProcess.Instance.add_log_line("Stabalising...");
            }
            
        }
    }
}
