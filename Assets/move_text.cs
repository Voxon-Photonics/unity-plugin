using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_text : MonoBehaviour {

    int step = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(step > 300)
        {
            gameObject.transform.position = new Vector3(Random.Range(-4.9f, 4.9f), Random.Range(-1.9f, 1.9f), Random.Range(-3.9f, 3.9f));
            Vector3 pos = gameObject.transform.position;
            pos.x += 0.5f;
            gameObject.transform.position = pos;
            Debug.Log("Base Location: " + gameObject.transform.position);
            gameObject.GetComponent<Voxon.VXTextComponent>().UpdateLocation();
            step = 0;
        }
        step++;
	    
	}
}
