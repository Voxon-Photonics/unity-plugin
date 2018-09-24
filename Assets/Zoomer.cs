using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Voxon.Input.GetKeyDown("Up"))
        {
            transform.localScale = transform.localScale - new Vector3(0.1f, 0.1f, 0.1f);
        }

        if (Voxon.Input.GetKeyDown("Down"))
        {
            transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
        }

    }
}
