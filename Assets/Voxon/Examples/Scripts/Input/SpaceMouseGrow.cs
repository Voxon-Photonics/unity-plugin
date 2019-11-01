using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMouseGrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Voxon.Input.GetSpaceNavButton("LeftButton") && transform.localScale.x < 5f)
        {
            transform.localScale += new Vector3(0.00005f,0.00005f, 0.00005f);
        }
        
        if (Voxon.Input.GetSpaceNavButton("RightButton") && transform.localScale.x > 0.00001f)
        {
            transform.localScale -= new Vector3(0.00005f,0.00005f, 0.00005f);
        }
    }
}
