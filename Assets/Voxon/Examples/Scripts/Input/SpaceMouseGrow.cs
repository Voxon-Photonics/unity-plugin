using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMouseGrow : MonoBehaviour
{
    public float scale_factor = 0.000005f; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Voxon.Input.GetSpaceNavButton("LeftButton") && transform.localScale.x < 0.0001f)
        {
            transform.localScale += new Vector3(scale_factor,scale_factor,scale_factor);
        }
        
        if (Voxon.Input.GetSpaceNavButton("RightButton") && transform.localScale.x > 0.00001f)
        {
            transform.localScale -= new Vector3(scale_factor,scale_factor,scale_factor);
        }
    }
}
