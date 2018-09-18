using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //gameObject.GetComponent<MeshRenderer>().material.mainTexture = TestObjects.tTexture();
        gameObject.GetComponent<MeshRenderer>().material = TestObjects.tMaterial();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
