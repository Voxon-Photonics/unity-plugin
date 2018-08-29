using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObj : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*
		if(Voxon.Input.GetKeyDown("Up"))
        {
            gameObject.tag = "VoxieHide";
        }

        if (Voxon.Input.GetKeyDown("Down"))
        {
            gameObject.tag = "Bob";
        }
        */

        if (Voxon.Input.GetKeyDown("Up"))
        {
            if (gameObject.tag == "VoxieHide")
            {
                Debug.Log("Visible");
                gameObject.tag = "Untagged";
            }
            else if (gameObject.tag == "Untagged")
            {
                Debug.Log("Hidden");
                gameObject.tag = "VoxieHide";
            }
        }
    }
}
