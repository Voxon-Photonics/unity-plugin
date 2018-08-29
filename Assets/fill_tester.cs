using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fill_tester : MonoBehaviour {
    Voxon.VXComponent VXComponent;
    FrameRate_Reporter Reporter;
	// Use this for initialization
	void Start () {
        VXComponent = GetComponent<Voxon.VXComponent>();
        Reporter = GetComponent<FrameRate_Reporter>();
    }
	
	// Update is called once per frame
	void Update () {

        if (VXComponent == null)
        {
            VXComponent = GetComponent<Voxon.VXComponent>();
        }

        if (Voxon.Input.GetKeyDown("Up"))
        {
            Vector3 pos = gameObject.transform.position;
            pos.y += 0.1f;
            gameObject.transform.position = pos;
            Reporter.Reset();
        }

        if (Voxon.Input.GetKeyDown("Down"))
        {
            Vector3 pos = gameObject.transform.position;
            pos.y -= 0.1f;
            gameObject.transform.position = pos;
            Reporter.Reset();
        }

        if (Voxon.Input.GetKeyDown("Dots"))
        {
            VXComponent.set_flag(0);
            Reporter.Reset();
        }

        if (Voxon.Input.GetKeyDown("Lines"))
        {
            VXComponent.set_flag(1);
            Reporter.Reset();
        }

        if (Voxon.Input.GetKeyDown("Surface"))
        {
            VXComponent.set_flag(2);
            Reporter.Reset();
        }

        if (Voxon.Input.GetKeyDown("Fill"))
        {
            VXComponent.set_flag(3);
            Reporter.Reset();
        }

    }
}
