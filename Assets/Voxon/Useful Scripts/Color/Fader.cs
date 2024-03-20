using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Fader a script to fade a game object in and out
 * Matthew Vecchio for Voxon
 *  Created 10/12/2018
 *  Updated 11/12/2018
 *  added isActive bool
 *  added setFade / getfade function
 *  added an overall brightness
 */
public class Fader : MonoBehaviour
{


    [Tooltip("Time in seconds to start the fade process 1 = 1 second")]
    public float startDelay = 1;
    float fadeTime = 0;
    [Tooltip("The amount of each fade (1.1 is 10% less than 2 is twice....")]
    public float fadeSpeed = 1.1f;

    [Tooltip("Time in seconds of next fade 1 = 1 second")]
    public float fadeDelay = .200f;
    [Tooltip("The minimal fade to show 1 is a full object 0 is transparent")]
    public float colourMinThreshold = .2f;
    [Tooltip("The maxium fade to show 1 is a full object 0 is transparent")]
    public float colourMaxThreshold = 1f;
    [Tooltip("The direction of fade tick = fade out untick = fade in")]
    public bool direction = true;
    [Tooltip("Option to enable or disable the fader (can be called by another script)")]
    public bool isOn = true;

    public float inactiveLevel = 1;

    [Tooltip("Adjust overall brightness")]
    public float brightnessAdjust = 0;


    [Tooltip("In seconds how long to stay at a max or min value")]
    public float maxStay = 2;
    Color tmpCol;
    // Use this for initialization
    void Start()
    {
        fadeTime = Time.time + startDelay;

    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            if (Time.time > fadeTime)
            {
                fadeTime = Time.time + fadeDelay;
                tmpCol = gameObject.GetComponent<Renderer>().sharedMaterial.color;
                if (direction)
                {

                    tmpCol.r /= fadeSpeed;
                    tmpCol.g /= fadeSpeed;
                    tmpCol.b /= fadeSpeed;


                }
                else
                {

                    tmpCol.r *= fadeSpeed;
                    tmpCol.g *= fadeSpeed;
                    tmpCol.b *= fadeSpeed;


                }

                if (tmpCol.g < colourMinThreshold || tmpCol.r < colourMinThreshold || tmpCol.b < colourMinThreshold)
                {
                    fadeTime = Time.time + maxStay;
                    direction = false;
                }
                else if (tmpCol.g > colourMaxThreshold || tmpCol.r > colourMaxThreshold || tmpCol.b > colourMaxThreshold)
                {
                    direction = true;
                    fadeTime = Time.time + maxStay;
                }

                tmpCol.r += brightnessAdjust;
                tmpCol.g += brightnessAdjust;
                tmpCol.b += brightnessAdjust;


            }
        }
        else
        {

            tmpCol.r = inactiveLevel + brightnessAdjust;
            tmpCol.g = inactiveLevel + brightnessAdjust;
            tmpCol.b = inactiveLevel + brightnessAdjust;

        }




        gameObject.GetComponent<Renderer>().sharedMaterial.color = tmpCol;
    }

    public Color getFade()
    {
        return gameObject.GetComponent<Renderer>().sharedMaterial.color;

    }

    public void setFade(Color tmpCol)
    {
        tmpCol.r += brightnessAdjust;
        tmpCol.g += brightnessAdjust;
        tmpCol.b += brightnessAdjust;

        gameObject.GetComponent<Renderer>().sharedMaterial.color = tmpCol;
    }

}
